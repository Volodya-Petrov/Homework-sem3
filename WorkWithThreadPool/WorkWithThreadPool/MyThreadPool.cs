using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace WorkWithThreadPool
{
    public class MyThreadPool
    {
        private ConcurrentQueue<Action> tasks;

        private Thread[] threads;

        private CancellationTokenSource _cancellationToken;
        
        public MyThreadPool(int countOfThreads)
        {
            _cancellationToken = new CancellationTokenSource();
            tasks = new ConcurrentQueue<Action>();
            threads = new Thread[countOfThreads];
            for (int i = 0; i < countOfThreads; i++)
            {
                threads[i] = CreateThread();
                threads[i].Start();
            }
        }

        public MyTask<T> Submit<T>(Func<T> task)
        {
            var myTask = new MyTask<T>(task);
            tasks.Enqueue(myTask.Run);
            return myTask;
        }

        public void Shutdown()
        {
            _cancellationToken.Cancel();
            while (threads.Any(t => t.IsAlive)) ;
        }
        
        private Thread CreateThread()
        {
            var thread = new Thread(() =>
            {
                while (true)
                {
                    if (_cancellationToken.Token.IsCancellationRequested)
                    {
                        return;
                    }
                    if (tasks.TryDequeue(out Action task))
                    {
                        task();
                    }
                }
            });
            thread.IsBackground = true;
            return thread;
        }
    }
}