using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MyNUnit
{
    public class MyNUnit
    {
        private string[] RunTestsFromDll(string path)
        {
            var classes = Assembly.LoadFrom(path).ExportedTypes.Where(t => t.IsClass);
            return null;
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
            List<MethodInfo> beforeClass, List<MethodInfo> afterClass, List<MethodInfo> tests, MethodInfo method)
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
                    beforeClass.Add(method);
                }

                if (attributeType == typeof(AfterClass))
                {
                    afterClass.Add(method);
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
                AddMethodInRightList(before, after, beforeClass, afterClass, tests, methodInfo);
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
            messagesForUser.Add($"Запуск тестов из класса {classFromDll.Name}");
            GetMethodsWithAttributes(before, after, beforeClass, afterClass, tests, messagesForUser, classFromDll);
            if (tests.Count == 0)
            {
                return null;
            }

            dynamic classInstance = Activator.CreateInstance(classFromDll);
            foreach (var method in beforeClass)
            {
                method.Invoke(null, null);
            }

            foreach (var test in tests)
            {
                var attribute = (Test)Attribute.GetCustomAttribute(test, typeof(Test));
                dynamic expected = Activator.CreateInstance(attribute.Expected);
                if (attribute.Ignore != null)
                {
                    continue;
                }

                foreach (var funcBefore in before)
                {
                    funcBefore.Invoke(classInstance, null);
                }

                try
                {
                    test.Invoke(classInstance, null);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            return null;
        }
    }
}