using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using WorkWithQueue;

namespace TestsForPriorityQueue
{
    public class Tests
    {
        private PriorityQueue queue;
        private Task[] tasks;
        
        [SetUp]
        public void Setup()
        {
            queue = new();
            tasks = new Task[5];
            for (int i = 0; i < 5; i++)
            {
                var index = i;
                tasks[i] = Task.Run(() =>
                {
                    for (int j = 5 * index; j < (index + 1) * 5; j++)
                    {
                        queue.Enqueue(j, j);
                    }
                });
            }
            Task.WhenAll(tasks).Wait();
        }

        [Test]
        public void TestForEnqueueWithMultithread()
        {
            Assert.AreEqual(25, queue.Size);
        }

        [Test]
        public void TestForPriorityQueueWorkCorrectly()
        {
            for (int i = 24; i >= 0; i--)
            {
                Assert.AreEqual(i, queue.Dequeue());
            }
        }

        [Test]
        public void TestForDequeueMultithreaded()
        {
            var newTasks = new Task<List<int>>[5];
            for (int i = 0; i < 5; i++)
            {
                newTasks[i] = Task.Run(() =>
                {
                    var list = new List<int>();
                    for (int i = 0; i < 5; i++)
                    {
                        list.Add(queue.Dequeue());
                    }
                    return list;
                });
            }

            Task.WhenAny(newTasks).Wait();
            var result = new List<int>();
            for (int i = 0; i < 5; i++)
            {
                result = result.Union(newTasks[i].Result).ToList();
            }
            Assert.AreEqual(25, result.Count);
            for (int i = 0; i < 25; i++)
            {
                Assert.IsTrue(result.Contains(i));
            }
        }

        [Test]
        public void TestForDequeueWhenZeroElements()
        {
            var newQueue = new PriorityQueue();
            var task = Task.Run(() => newQueue.Dequeue());
            var task1 = Task.Run(() =>
            {
                Thread.Sleep(100);
                newQueue.Enqueue(5, 5);
            });
            Assert.AreEqual(5, task.Result);
        }
    }
}