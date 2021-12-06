using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using MyNUnit;

namespace TestForMyNUnit
{
    public class Tests
    {
        private MyNUnit.MyNUnit myNUnit = new MyNUnit.MyNUnit();
    

        [TestCaseSource(nameof(MessagesThatShouldBe))]
        [NUnit.Framework.Test]
        public void TestForMessagesThatShouldPrintToUser(string message)
        {
            var result = myNUnit.RunTests("../../../../TestProject/bin/Debug/net5.0/");
            Assert.IsTrue(result.Contains(message));
        }

        [NUnit.Framework.Test]
        public void TestForGeneralState()
        {
            var result = myNUnit.RunTests("../../../../TestProject/bin/Debug/net5.0/");
            Assert.AreEqual(11, result.Length);
        }
        
        private static IEnumerable<string> MessagesThatShouldBe()
        {
            yield return "Тест TestWithoutExpected прошел успешно";
            yield return "Тест TestWithExpectedException прошел успешно";
            yield return "Тест TestBefore прошел успешно";
            yield return "Метод NonStaticBeforeClass содержит атрибут BeforeClass, но не является статическим";
            yield return
                "Тест ExceptionExpectedButWasNull провален: ожидалось исключения типа System.ArgumentException";
            yield return
                "Тест OneExceptionExpectedButWasAnother провален: ожидалось исключения типа System.ArgumentException, возникло System.AggregateException";
            yield return "Тест NullExpectedButThrowException провален: возникло исключение System.ArgumentException";
            yield return "Метод TestWithIncompatibleAttributes имеет два несовместимых атрибута";
            yield return "В методе ExceptionInAfterClass возникло исключение: System.AggregateException";
            yield return "Запуск тестов из класса ForCorrectTests";
            yield return "Запуск тестов из класса ForIncorrectTests";
        }
    }
}