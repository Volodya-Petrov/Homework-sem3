using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MyNUnit
{
    public class MyNUnit
    {
        public string[] RunTests(string path)
        {
            var allDllFiles = Directory.GetFiles(path, "*.dll");
            var tasks = new Task<string[]>[allDllFiles.Length];
            for (int i = 0; i < allDllFiles.Length; i++)
            {
                var index = i;
                tasks[i] = Task.Run(() => RunTestsFromDll(allDllFiles[index]));
            }

            Task.WhenAll(tasks).Wait();
            var infoAboutTests = new string[1];
            for (int i = 0; i < tasks.Length; i++)
            {
                infoAboutTests.Concat(tasks[i].Result);
            }

            return infoAboutTests;
        }
        
        private string[] RunTestsFromDll(string path)
        {
            var messages = new string[1];
            var classes = Assembly.LoadFrom(path).ExportedTypes.Where(t => t.IsClass);
            foreach (var exportCLass in classes)
            {
                var infoAboutTests = RunTestsFromClass(exportCLass);
                messages.Concat(infoAboutTests);
            }
            return messages;
        }

        private bool MethodHaveIncompatibleAttributes(MethodInfo method)
        {
            var countOfAttributes = 0;
            foreach (var attribute in method.CustomAttributes)
            {
                var type = attribute.GetType();
                if (type == typeof(Test) || type == typeof(Before) || type == typeof(After) ||
                    type == typeof(BeforeClass) || type == typeof(AfterClass))
                {
                    countOfAttributes++;
                }
            }

            return countOfAttributes > 1;
        }

        private void AddMethodInRightList(List<MethodInfo> before, List<MethodInfo> after,
            List<MethodInfo> beforeClass, List<MethodInfo> afterClass, List<MethodInfo> tests, List<string> errors, MethodInfo method)
        {
            foreach (var attributes in method.CustomAttributes)
            {
                var attributeType = attributes.GetType();
                if (attributeType == typeof(Test))
                {
                    tests.Add(method);
                }

                if (attributeType == typeof(After))
                {
                    after.Add(method);
                }

                if (attributeType == typeof(Before))
                {
                    before.Add(method);
                }

                if (attributeType == typeof(BeforeClass))
                {
                    if (method.IsStatic)
                    {
                        beforeClass.Add(method);
                    }
                    else
                    {
                        errors.Add($"Метод {method.Name} содержит атрибут BeforeClass, но не является статическим");
                    }
                }

                if (attributeType == typeof(AfterClass))
                {
                    if (method.IsStatic)
                    {
                        afterClass.Add(method);
                    }
                    else
                    {
                        errors.Add($"Метод {method.Name} содержит атрибут AfterClass, но не является статическим");
                    }
                }
            }
        }

        private void GetMethodsWithAttributes(List<MethodInfo> before, List<MethodInfo> after,
            List<MethodInfo> beforeClass, List<MethodInfo> afterClass, List<MethodInfo> tests, List<string> messages, Type classFromDll)
        {
            foreach (var methodInfo in classFromDll.GetMethods())
            {
                if (MethodHaveIncompatibleAttributes(methodInfo))
                {
                    messages.Add($"Метод {methodInfo.Name} имеет два несовместимых атрибута");
                    continue;
                }
                AddMethodInRightList(before, after, beforeClass, afterClass, tests, messages, methodInfo);
            }
        }

        private void RunMethods(List<MethodInfo> methods, dynamic classInstance, List<string> bugsReport)
        {
            foreach (var method in methods)
            {
                try
                {
                    method.Invoke(classInstance, null);
                }
                catch (Exception e)
                {
                    bugsReport.Add($"В методе {method.Name} возникло исключение: {e.Message}");
                }
            }
        }

        private void RunTest(MethodInfo test, dynamic classInstance, Type expected, List<string> messagesForUser)
        {
            try
            {
                test.Invoke(classInstance, null);
            }
            catch (Exception exception)
            {
                if (expected == null)
                {
                    messagesForUser.Add($"Тест {test.Name} провален: возникло исключение {exception.Message}");
                }
                else if (exception.GetType() != expected)
                {
                    messagesForUser.Add(
                        $"Тест {test.Name} провален: ожидалось исключения типа {expected}, возникло {exception.GetType()} ");
                }
                else
                {
                    messagesForUser.Add($"Тест {test.Name} прошел успешно");
                }
            }
            finally
            {
                if (expected == null)
                {
                    messagesForUser.Add($"Тест {test.Name} прошел успешно");
                }
                else
                {
                    messagesForUser.Add($"Тест {test.Name} провален: ожидалось исключения типа {expected}");
                }
            }
        }
        
        private string[] RunTestsFromClass(Type classFromDll)
        {
            var after = new List<MethodInfo>();
            var before = new List<MethodInfo>();
            var beforeClass = new List<MethodInfo>();
            var afterClass = new List<MethodInfo>();
            var tests = new List<MethodInfo>();
            var messagesForUser = new List<string>();
            GetMethodsWithAttributes(before, after, beforeClass, afterClass, tests, messagesForUser, classFromDll);
            if (tests.Count == 0)
            {
                return messagesForUser.ToArray();
            }
            
            messagesForUser.Add($"Запуск тестов из класса {classFromDll.Name}");

            dynamic classInstance = Activator.CreateInstance(classFromDll);
            RunMethods(beforeClass, null, messagesForUser);
            
            foreach (var test in tests)
            {
                var attribute = (Test)Attribute.GetCustomAttribute(test, typeof(Test));
                if (attribute.Ignore != null)
                {
                    continue;
                }
                
                RunMethods(before, classInstance, messagesForUser);
                RunTest(test, classInstance, attribute.Expected, messagesForUser);
                RunMethods(after, classInstance, messagesForUser);
            }
            
            RunMethods(afterClass, null, messagesForUser);

            return messagesForUser.ToArray();
        }
    }
}