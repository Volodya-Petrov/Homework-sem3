using System;
using MyNUnit;

namespace TestProject
{
    public class ForCorrectTests
    {
        private int nonStaticCounter = 0;

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
        public void TestBefore()
        {
            if (nonStaticCounter != 1)
            {
                throw new ArgumentException();
            }
        }
    }
}