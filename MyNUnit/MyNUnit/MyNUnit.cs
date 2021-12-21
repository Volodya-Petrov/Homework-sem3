using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AttributesForMyNUnit;

namespace MyNUnit
{
    /// <summary>
    /// Класс для запуска тестов
    /// </summary>
    public class MyNUnit
    {   
        private ConcurrentBag<TestInfo> result;
        
        /// <summary>
        /// Запускает тесты из dll файлов по заданной директории
        /// </summary>
        public TestInfo[] RunTests(string path)
        {
            var allDllFiles = Directory.GetFiles(path, "*.dll");
            Parallel.ForEach(allDllFiles, path => RunTestsFromDll(path));
            return result.ToArray();
        }
        
        private void RunTestsFromDll(string path)
        {
            var classes = Assembly.LoadFrom(path).ExportedTypes.Where(t => t.IsClass);
            Parallel.ForEach(classes, c => RunTestsFromClass(c));
        }

        private bool MethodHaveIncompatibleAttributes(MethodInfo method)
        {
            var countOfAttributes = 0;
            foreach (var attribute in method.CustomAttributes)
            {
                var type = attribute.AttributeType;
                if (type == typeof(Test) || type == typeof(Before) || type == typeof(After) ||
                    type == typeof(BeforeClass) || type == typeof(AfterClass))
                {
                    countOfAttributes++;
                }
            }

            return countOfAttributes > 1;
        }

        private bool MethodHaveReturnTypeOrParametrs(MethodInfo method)
        {
            return method.ReturnType.Name != "Void" || method.GetParameters().Length > 0;
        }

        private void AddMethodInRightList(ListsWithMethods methods, MethodInfo method)
        {
            foreach (var attributes in method.CustomAttributes)
            {
                var attributeType = attributes.AttributeType;
                if (attributeType == typeof(Test))
                {
                    methods.Tests.Add(method);
                }

                if (attributeType == typeof(After))
                {
                    methods.After.Add(method);
                }

                if (attributeType == typeof(Before))
                {
                    methods.Before.Add(method);
                }

                if (attributeType == typeof(BeforeClass))
                {
                    methods.BeforeClass.Add(method);
                }

                if (attributeType == typeof(AfterClass))
                {
                    methods.AfterClass.Add(method);
                }
            }
        }

        private void GetMethodsWithAttributes(ListsWithMethods methods, Type classFromDll)
        {
            foreach (var methodInfo in classFromDll.GetMethods())
            {
                AddMethodInRightList(methods, methodInfo);
            }
        }

        private bool CheckMethodIsCorrect(MethodInfo method, bool isStatic, out string errorMessage)
        {
            if (MethodHaveIncompatibleAttributes(method))
            {
                errorMessage = $"Метод {method.Name} имеет несовместимые атрибуты";
                return false;
            }

            if (method.IsStatic != isStatic)
            {
                errorMessage = isStatic
                    ? $"Метод {method.Name} должен быть статическим"
                    : $"Метод {method.Name} не должен быть статическим";
                return false;
            }

            if (MethodHaveReturnTypeOrParametrs(method))
            {
                errorMessage = $"Метод {method.Name} имеет возвращаемое значение или принимает параметры";
                return false;
            }

            errorMessage = null;
            return true;
        }
        
        private bool RunMethods(List<MethodInfo> methods, object classInstance, bool isStatic, out string errorMessage)
        {
            foreach (var method in methods)
            {
                if (!CheckMethodIsCorrect(method, isStatic, out errorMessage))
                {   
                    return false;
                }

                try
                {
                    method.Invoke(classInstance, null);
                }
                catch (Exception e)
                {   
                    errorMessage = $"В методе {method.Name} возникло исключение: {e.InnerException.GetType()}";
                    return false;
                }
            }

            errorMessage = null;
            return true;
        }

        private void RunTest(MethodInfo test, object classInstance, List<MethodInfo> before, List<MethodInfo> after)
        {
            var testInfo = new TestInfo();
            var attribute = (Test)Attribute.GetCustomAttribute(test, typeof(Test));
            if (attribute.Ignore != null)
            {
                testInfo.State = TestState.Ignored;
                testInfo.IgnoreMessage = attribute.Ignore;
                result.Add(testInfo);
                return;
            }

            if (!RunMethods(before, classInstance, false, out string errorMessage))
            {
                testInfo.State = TestState.Errored;
                testInfo.ErrorMessage = errorMessage;
                result.Add(testInfo);
                return;
            }

            if (!CheckMethodIsCorrect(test, false, out errorMessage))
            {
                testInfo.State = TestState.Failed;
                testInfo.ErrorMessage = errorMessage;
                result.Add(testInfo);
                return;
            }

            var expected = attribute.Expected;
            var message = "";
            var state = TestState.Success;
            var timer = new Stopwatch();
            timer.Start();
            try
            {
                test.Invoke(classInstance, null);
            }
            catch (Exception exception)
            {
                if (expected == null)
                {
                    message = $"Тест {test.Name} провален: возникло исключение {exception.InnerException.GetType()}";
                    state = TestState.Failed;
                }
                else if (exception.InnerException.GetType() != expected)
                {
                    message = $"Тест {test.Name} провален: ожидалось исключения типа {expected}, возникло {exception.InnerException.GetType()}";
                    state = TestState.Failed;
                }
                else
                {
                    message = $"Тест {test.Name} прошел успешно";
                    state = TestState.Success;
                }
            }
            finally
            {
                if (message == "")
                {
                    if (expected == null)
                    {
                        message = $"Тест {test.Name} прошел успешно";
                        state = TestState.Success;
                    }
                    else
                    {
                        message = $"Тест {test.Name} провален: ожидалось исключения типа {expected}";
                        state = TestState.Failed;
                    }
                }
            }
            timer.Stop();
            testInfo.State = state;
            testInfo.ErrorMessage = message;
            testInfo.Time = timer.ElapsedMilliseconds;
            if (!RunMethods(after, classInstance, false, out errorMessage))
            {
                testInfo.State = TestState.Errored;
                testInfo.ErrorMessage = errorMessage;
                result.Add(testInfo);
                return;
            }
            result.Add(testInfo);
        }

        private void MakeAllTestsFromClassErrored(string className, List<MethodInfo> tests, string errorMessage)
        {
            for (int i = 0; i < tests.Count; i++)
            {
                result.Add(new TestInfo()
                {
                    ErrorMessage = errorMessage,
                    ClassName = className,
                    IgnoreMessage = "",
                    Name = tests[i].Name,
                    State = TestState.Errored
                });
            }
        }
        
        private void RunTestsFromClass(Type classFromDll)
        {
            var methods = new ListsWithMethods();
            var messagesForUser = new List<string>();
            GetMethodsWithAttributes(methods, classFromDll);
            if (!RunMethods(methods.BeforeClass, null, true, out string errorMessage))
            {
                MakeAllTestsFromClassErrored(classFromDll.Name, methods.Tests, errorMessage);
                return;
            }
            Parallel.ForEach(methods.Tests, test =>
            {
                object classInstanse = Activator.CreateInstance(classFromDll);
                RunTest(test, classInstanse, methods.Before, methods.After);
            });
            if (!RunMethods(methods.AfterClass, null, true, out errorMessage))
            {
                MakeAllTestsFromClassErrored(classFromDll.Name, methods.Tests, errorMessage);
            }
        }
        
        private class ListsWithMethods
        {
            public ListsWithMethods()
            {
                After = new();
                Before = new();
                BeforeClass = new();
                AfterClass = new();
                Tests = new();
            }
            
            public List<MethodInfo> After { get; set; }
            public List<MethodInfo> Before { get; set; }
            public List<MethodInfo> BeforeClass { get; set; }
            public List<MethodInfo> AfterClass { get; set; }
            public List<MethodInfo> Tests { get; set; }
        }
    }
}