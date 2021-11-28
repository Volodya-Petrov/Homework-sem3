using System;
using MyNUnit;

namespace TestForMyNUnit
{
    public class ForTests
    {
        [Test(null)]
        public void CorrectTest()
        {
        }

        [Test(null, "yes")]
        public void TestShouldBeIgnored()
        {
            
        }

        [Test(typeof(ArgumentException))]
        public void CorrectTestWithExpectedException()
        {
            throw new AggregateException();
        }
    }
}