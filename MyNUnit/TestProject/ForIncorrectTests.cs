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

        [BeforeClass]
        public void NonStaticBeforeClass()
        {
            
        }

        [AfterClass]
        public static void ExceptionInAfterClass()
        {
            throw new AggregateException();
        }
        
        [After]
        [Test(null)]
        public void TestWithIncompatibleAttributes()
        {
            
        }
    }
}