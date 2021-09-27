using System;
using NUnit.Framework;
using WorkWithLazy;

namespace TestsForLazy
{
    public class Tests
    {
        private int numberFunctionLaunches = 0;
        private Func<string> function;
        
        [SetUp]
        public void Setup()
        {
            function = () =>
            {
                numberFunctionLaunches++;
                return "plug";
            };
        }

        [Test]
        public void TestLazyLaunchFunctionOnly1Time()
        {
            var lazy = LazyFactory<string>.CreateLazy(function);
            for (int i = 0; i < 100; i++)
            {
                Assert.AreEqual("plug", lazy.Get());
                Assert.AreEqual(1, numberFunctionLaunches);
            }
        }

        [Test]
        public void TestLazyShouldThrowArgumentExceptionWhenNullFunctionWasSent()
            => Assert.Throws<ArgumentException>(() => LazyFactory<int>.CreateLazy(null));
    }
}