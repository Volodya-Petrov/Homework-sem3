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

        private int countOfTasks = 20;
        
        [SetUp]
        public void Setup()
        {
            threadPool = new MyThreadPool(5);
            tasks = new IMyTask<int>[countOfTasks];
            functions = new Func<int>[countOfTasks];
            for (int i = 0; i < countOfTasks; i++)
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

            for (int i = 0; i < countOfTasks; i++)
            {
                tasks[i] = threadPool.Submit(functions[i]);
            }
        }
        
        [Test]
        public void TestThreadPoolShouldSolveTasksCorrectly()
        {
            for (int i = 0; i < countOfTasks; i++)
            {
                Assert.AreEqual(answerForFuncs + i, tasks[i].Result);
            }
        }
        
        
        [Test]
        public void TestAfterShutDownShouldCalculateSubmittedTasks()
        {
            threadPool.Shutdown();
            for (int i = 0; i < countOfTasks; i++)
            {
                Assert.AreEqual(answerForFuncs + i, tasks[i].Result);
            }
        }

        [Test]
        public void TestAfterShutDownUCantSubmitNewTask()
        {
            threadPool.Shutdown();
            Assert.Throws<InvalidOperationException>(() => threadPool.Submit(functions[0]));
        }

        [Test]
        public void TestContinueWithShouldBeCalculatedCorrectly()
        {
            var continueTasks = new IMyTask<string>[countOfTasks];
            for (int i = 0; i < countOfTasks; i++)
            {
                continueTasks[i] = tasks[i].ContinueWith(intToString);
            }

            for (int i = 0; i < countOfTasks; i++)
            {
                Assert.AreEqual((answerForFuncs + i).ToString(), continueTasks[i].Result);
            }
        }
        
        [Test]
        public void TestAfterShutdownSubmittedContinueWithShouldBeCalculatedCorrectly()
        {
            var continueTasks = new IMyTask<string>[countOfTasks];
            for (int i = 0; i < countOfTasks; i++)
            {
                continueTasks[i] = tasks[i].ContinueWith(intToString);
            }
            threadPool.Shutdown();
            for (int i = 0; i < countOfTasks; i++)
            {
                Assert.AreEqual((answerForFuncs + i).ToString(), continueTasks[i].Result);
            }
        }
        
        [Test]
        public void TestAfterShutDownUCantSubmitContinueWith()
        {
            threadPool.Shutdown();
            Assert.Throws<InvalidOperationException>(() => tasks[0].ContinueWith(intToString));
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

        [Test]
        public void TestWithSeveralThreads()
        {
            var threads = new Thread[10];
            for (int i = 0; i < 10; i++)
            {
                threads[i] = new Thread(() =>
                {
                    var threadTasks = new IMyTask<int>[countOfTasks];
                    var threadFunctions = new Func<int>[countOfTasks];
                    for (int j = 0; j < countOfTasks; j++)
                    {
                        var index = j;
                        threadFunctions[j] = new Func<int>(() =>
                        {
                            var result = 0;
                            for (int z = 0; z < answerForFuncs; z++)
                            {
                                result++;
                            }

                            return result + index;
                        });
                    }

                    for (int j = 0; j < countOfTasks; j++)
                    {
                        threadTasks[j] = threadPool.Submit(functions[j]);
                    }

                    for (int j = 0; j < countOfTasks; j++)
                    {
                        Assert.AreEqual(answerForFuncs + j, threadTasks[j].Result);
                    }
                });
            }

            for (int i = 0; i < 10; i++)
            {
                threads[i].Start();
                threads[i].Join();
            }
        }
    }
}