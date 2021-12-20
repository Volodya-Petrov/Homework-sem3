using System;
using System.Collections.Generic;
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
        /// <summary>
        /// Запускает тесты из dll файлов по заданной директории
        /// </summary>
        public string[] RunTests(string path)
        {
            var allDllFiles = Directory.GetFiles(path, "*.dll");
            var tasks = new Task<List<string>>[allDllFiles.Length];
            for (int i = 0; i < allDllFiles.Length; i++)
            {
                var index = i;
                tasks[i] = Task.Run(() => RunTestsFromDll(allDllFiles[index]));
            }
            
            var infoAboutTests = new List<string>();
            for (int i = 0; i < tasks.Length; i++)
            {
                infoAboutTests = infoAboutTests.Union(tasks[i].Result).ToList();
            }

            if (infoAboutTests.Count == 0)
            {
                return new string[] { "По заданному пути не было найдено тестов" };
            }
            return infoAboutTests.ToArray();
        }
        
        private List<string> RunTestsFromDll(string path)
        {
            var messages = new List<string>();
            var classes = Assembly.LoadFrom(path).ExportedTypes.Where(t => t.IsClass);
            foreach (var exportClass in classes)
            {
                var infoAboutTests = RunTestsFromClass(exportClass);
                messages = messages.Union(infoAboutTests).ToList();
            }
            return messages;
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

        private void AddMethodInRightList(List<MethodInfo> before, List<MethodInfo> after,
            List<MethodInfo> beforeClass, List<MethodInfo> afterClass, List<MethodInfo> tests, List<string> errors, MethodInfo method)
        {
            foreach (var attributes in method.CustomAttributes)
            {
                var attributeType = attributes.AttributeType;
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

        private void RunMethods(List<MethodInfo> methods, object classInstance, List<string> bugsReport)
        {
            foreach (var method in methods)
            {
                try
                {
                    method.Invoke(classInstance, null);
                }
                catch (Exception e)
                {
                    bugsReport.Add($"В методе {method.Name} возникло исключение: {e.InnerException.GetType()}");
                }
            }
        }

        private void RunTest(MethodInfo test, object classInstance, Type expected, List<string> messagesForUser)
        {
            var message = "";
            try
            {
                test.Invoke(classInstance, null);
            }
            catch (Exception exception)
            {
                if (expected == null)
                {
                    message = $"Тест {test.Name} провален: возникло исключение {exception.InnerException.GetType()}";
                }
                else if (exception.InnerException.GetType() != expected)
                {
                    message = $"Тест {test.Name} провален: ожидалось исключения типа {expected}, возникло {exception.InnerException.GetType()}";
                }
                else
                {
                    message = $"Тест {test.Name} прошел успешно";
                }
            }
            finally
            {
                if (message == "")
                {
                    if (expected == null)
                    {
                        message = $"Тест {test.Name} прошел успешно";
                    }
                    else
                    {
                        message = $"Тест {test.Name} провален: ожидалось исключения типа {expected}";
                    }
                }
            }
            messagesForUser.Add(message);
        }
        
        private List<string> RunTestsFromClass(Type classFromDll)
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
                return messagesForUser;
            }
            
            messagesForUser.Add($"Запуск тестов из класса {classFromDll.Name}");
            
            RunMethods(beforeClass, null, messagesForUser);
            var tasks = new Task<List<string>>[tests.Count];
            for (int i = 0; i < tests.Count; i++)
            {   
                var test = tests[i];
                tasks[i] = Task.Run(() =>
                {
                    var messagesFromCurrentTest = new List<string>();
                    var attribute = (Test)Attribute.GetCustomAttribute(test, typeof(Test));
                    if (attribute.Ignore != null)
                    {
                        return messagesFromCurrentTest;
                    }
                    object classInstance = Activator.CreateInstance(classFromDll);    
                    RunMethods(before, classInstance, messagesFromCurrentTest);
                    RunTest(test, classInstance, attribute.Expected, messagesFromCurrentTest);
                    RunMethods(after, classInstance, messagesFromCurrentTest);
                    return messagesFromCurrentTest;
                });
            }
            RunMethods(afterClass, null, messagesForUser);
            foreach (var task in tasks)
            {
                messagesForUser = messagesForUser.Union(task.Result).ToList();
            }
            return messagesForUser;
        }
    }
}