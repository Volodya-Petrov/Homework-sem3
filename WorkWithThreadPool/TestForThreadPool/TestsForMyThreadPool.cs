using System;
using System.Threading;
using NUnit.Framework;
using WorkWithThreadPool;

namespace TestForThreadPool
{
    public class Tests
    {
        private readonly int answerForFuncs = 10000000;
        
        private Func<int, string> intToString = x => x.ToString();

        private Func<int>[] functions;

        private IMyTask<int>[] tasks;

        private MyThreadPool threadPool;
        
        [SetUp]
        public void Setup()
        {
            threadPool = new MyThreadPool(5);
            tasks = new IMyTask<int>[10];
            functions = new Func<int>[10];
            for (int i = 0; i < 10; i++)
            {
                var index = i;
                functions[i] = new Func<int>(() =>
                {
                    var result = 0;
                    for (int j = 0; j < answerForFuncs; j++)
                    {
                        result++;
                    }

                    return result + index;
                });
            }

            for (int i = 0; i < 10; i++)
            {
                tasks[i] = threadPool.Submit(functions[i]);
            }
        }

        [Test]
        public void TestThreadPoolShouldSolveTasksCorrectly()
        {
            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(answerForFuncs + i, tasks[i].Result);
            }
        }
        
        [Test]
        public void TestAfterShoutDownShouldCalculateSubmittedTasks()
        {
            threadPool.Shutdown();
            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(answerForFuncs + i, tasks[i].Result);
            }
        }

        [Test]
        public void TestAfterShoutDownUCantSumbitNewTask()
        {
            threadPool.Shutdown();
            Assert.Throws<ThreadInterruptedException>(() => threadPool.Submit(functions[0]));
        }

        [Test]
        public void TestContinueWithShouldBeCalculatedCorrectly()
        {
            IMyTask<string>[] continueTasks = new IMyTask<string>[10];
            for (int i = 0; i < 10; i++)
            {
                continueTasks[i] = tasks[i].ContinueWith(intToString);
            }

            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual((answerForFuncs + i).ToString(), continueTasks[i].Result);
            }
        }
        
        [Test]
        public void TestAfterShoutdownSubmittedContinueWithShouldBeCalculatedCorrectly()
        {
            IMyTask<string>[] continueTasks = new IMyTask<string>[10];
            for (int i = 0; i < 10; i++)
            {
                continueTasks[i] = tasks[i].ContinueWith(intToString);
            }
            threadPool.Shutdown();
            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual((answerForFuncs + i).ToString(), continueTasks[i].Result);
            }
        }
        
        [Test]
        public void TestAfterShoutDownUCantSumbitContinueWith()
        {
            threadPool.Shutdown();
            Assert.Throws<ThreadInterruptedException>(() => tasks[0].ContinueWith(intToString));
        }

        [Test]
        public void TestTaskResultShouldThrowExceptions()
        {
            var func = new Func<int>(() =>
            {
                throw new Exception();
                return 1;
            });
            var task = threadPool.Submit(func);
            var continueTask = task.ContinueWith(intToString);
            Assert.Throws<AggregateException>(() =>
            {
                var test = task.Result;
            });
            Assert.Throws<AggregateException>(() =>
            {
                var test = continueTask.Result;
            });
        }
    }
}