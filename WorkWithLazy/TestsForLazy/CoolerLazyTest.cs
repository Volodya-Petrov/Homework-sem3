using System;
using System.Threading;
using NUnit.Framework;
using WorkWithLazy;

namespace TestsForLazy
{
    public class CoolerLazyTests
    {
        private int _checkForRacing;
        private int _numberOfLaunches;
        private Thread[] _threads;
        
        [SetUp]
        public void Setup()
        {
            var veryCoolLazy = LazyFactory<int>.CreateCoolerLazy(() =>
            {
                Interlocked.Increment(ref _numberOfLaunches);
                for (int i = 0; i < 10000; i++)
                {
                    Interlocked.Increment(ref _checkForRacing);
                }
                return _checkForRacing;
            });
            _threads = new Thread[Environment.ProcessorCount];
            for (int i = 0; i < _threads.Length; i++)
            {
                _threads[i] = new Thread(() =>
                {
                    veryCoolLazy.Get();
                });
            }
        }

        [Test]
        public void TestForRacingAndCalculationOnly1Time()
        {
            foreach (var thread in _threads)
            {
                thread.Start();
            }

            foreach (var thread in _threads)
            {
                thread.Join();
            }
            Assert.AreEqual(1, _numberOfLaunches);
            Assert.AreEqual(10000, _checkForRacing);
        }

        [Test]
        public void CoolerLazyShouldThrowExceptionWhenNullFunctionWasSent()
            => Assert.Throws<ArgumentException>(() => LazyFactory<int>.CreateCoolerLazy(null));
    }
}