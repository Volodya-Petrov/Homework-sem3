using System;
using MyNUnit;

namespace TestForMyNUnit
{
    public class ForCorrectTest
    {
        private static int counter = 0;
        private int nonStaticCounter = 0;
        
        [BeforeClass]
        public static void Increment() => counter++;

        [Before]
        public void NonStaticIncrement() => nonStaticCounter++;

        [Test(null)]
        public void TestWithoutExpected()
        {
            
        }

        [Test(null, "yes")]
        public void TestShouldBeIgnored()
        {
            
        }

        [Test(typeof(ArgumentException))]
        public void TestWithExpectedException()
        {
            throw new ArgumentException();
        }
        
        [Test(null)]
        public void TestBeforeClass()
        {
            if (counter != 1)
            {
                throw new ArgumentException();
            }
        }

        [Test(null)]
        public void TestBefore()
        {
            if (nonStaticCounter != 1)
            {
                throw new ArgumentException();
            }
        }
    }
}