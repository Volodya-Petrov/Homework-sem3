using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using MyNUnit;

namespace TestForMyNUnit
{
    public class Tests
    {
        private MyNUnit.MyNUnit myNUnit = new ();
    

        [TestCaseSource(nameof(MessagesThatShouldBe))]
        [Test]
        public void TestForMessagesThatShouldPrintToUser(TestInfo testInfo)
        {
            var result = myNUnit.RunTests("../../../../TestProject/bin/Debug/net5.0/");
            Assert.IsTrue(result.FirstOrDefault(ti => ti.Name == testInfo.Name && ti.ClassName == testInfo.ClassName && ti.State == testInfo.State) != null);
        }

        [Test]
        public void TestForGeneralState()
        {
            var result = myNUnit.RunTests("../../../../TestProject/bin/Debug/net5.0/");
            Assert.AreEqual(11, result.Length);
        }

        private static IEnumerable<TestInfo> MessagesThatShouldBe()
        {
            yield return new TestInfo()
                {ClassName = "ForCorrectTests", Name = "TestShouldBeIgnored", State = TestState.Ignored};
            var nameOfTestsFromCorrectTests = new string[]
            {
                "TestWithoutExpected", "TestWithExpectedException", "TestBefore"
            };
            foreach (var testName in nameOfTestsFromCorrectTests)
            {
                yield return new TestInfo() {ClassName = "ForCorrectTests", Name = testName, State = TestState.Success};
            }

            var nameOfTestsThatShouldBeFailed = new string[]
            {
                "NullExpectedButThrowException", "ExceptionExpectedButWasNull", "OneExceptionExpectedButWasAnother"
            };
            foreach (var testName in nameOfTestsThatShouldBeFailed)
            {
                yield return new TestInfo()
                    {ClassName = "ForIncorrectTests", Name = testName, State = TestState.Failed};
            }
            yield return new TestInfo() {ClassName = "ForIncorrectTests", Name = "StaticTest", State = TestState.Errored};
            yield return new TestInfo() {ClassName = "ForIncorrectTests", Name = "TestWithReturnValue", State = TestState.Errored};
            yield return new TestInfo() {ClassName = "ForIncorrectTests", Name = "TestWithParameters", State = TestState.Errored};
            yield return new TestInfo() {ClassName = "ForIncorrectTestsWithBeforeClass", Name = "Test", State = TestState.Errored};
        }
    }
}