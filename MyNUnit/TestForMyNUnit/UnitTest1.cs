using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using MyNUnit;
using MyNUnit;

namespace TestForMyNUnit
{
    public class Tests
    {
        private MyNUnit.MyNUnit myNUnit = new MyNUnit.MyNUnit();


        [TestCaseSource(nameof(MessagesThatShouldBe))]
        [NUnit.Framework.Test]
        public void CorrectTestWithoutExpected(string message)
        {
            var result = myNUnit.RunTests("./");
            Assert.IsTrue(result.Contains(message));
        }
        
        private static IEnumerable<string> MessagesThatShouldBe()
        {
            yield return "Тест TestWithoutExpected прошел успешно";
            yield return "Тест TestWithExpectedException прошел успешно";
            yield return "Тест TestBeforeClass прошел успешно";
            yield return "Тест TestBefore прошел успешно";
            yield return "Метод NonStaticBeforeClass содержит атрибут BeforeClass, но не является статическим";
            yield return
                "Тест ExceptionExpectedButWasNull провален: ожидалось исключения типа System.ArgumentException";
            yield return
                "Тест OneExceptionExpectedButWasAnother провален: ожидалось исключения типа System.ArgumentException, возникло System.AggregateException";
            yield return "Тест NullExpectedButThrowException провален: возникло исключение System.ArgumentException";
            yield return "Метод TestWithIncompatibleAttributes имеет два несовместимых атрибута";
            yield return "В методе ExceptionInAfterClass возникло исключение: System.AggregateException";
        }
    }
}