using System;
using AttributesForMyNUnit;

namespace TestProject
{
    public class ForIncorrectTestsWithBeforeClass
    {
        [BeforeClass]
        public void MethodThrowException()
        {
            throw new AggregateException();
        }

        [Test(null)]
        public void Test()
        {
            
        }
    }
}