using System;
using System.Threading;

namespace WorkWithThreadPool
{
    class Program
    {
        static void Main(string[] args)
        {
            var tasks = new MyTask<int>[10];
            var threadPool = new MyThreadPool(5);
            for (int i = 0; i < 10; i++)
            {
                var func = new Func<int>(() =>
                {
                    var result = 0;
                    for (int j = 0; j < 10000000; j++)
                    {
                        result++;
                    }

                    return result;
                });
                tasks[i] = threadPool.Submit(func);
            }
            threadPool.Shutdown();
            Thread.Sleep(1000);
        }
    }
}