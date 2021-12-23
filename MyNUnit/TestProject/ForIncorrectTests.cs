using System;
using AttributesForMyNUnit;
using MyNUnit;

namespace TestProject
{
    public class ForIncorrectTests
    {
        [Test(null)]
        public void NullExpectedButThrowException()
        {
            throw new ArgumentException();
        }

        [Test(typeof(ArgumentException))]
        public void ExceptionExpectedButWasNull()
        {
            
        }

        [Test(typeof(ArgumentException))]
        public void OneExceptionExpectedButWasAnother()
        {
            throw new AggregateException();
        }

        [Test(null)]
        public static void StaticTest()
        {
            
        }

        [Test(null)]
        public int TestWithReturnValue()
        {
            return 1;
        }

        [Test(null)]
        public void TestWithParameters(int a)
        {
            
        }
    }
}