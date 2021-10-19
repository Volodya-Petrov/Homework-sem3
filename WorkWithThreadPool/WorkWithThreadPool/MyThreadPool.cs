using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace WorkWithThreadPool
{   
    /// <summary>
    /// класс для параллельных исполнений задач
    /// </summary>
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
        
        /// <summary>
        /// добавляет задачу в очередь на исполнение
        /// </summary>
        public MyTask<T> Submit<T>(Func<T> task)
        {
            if (_cancellationToken.Token.IsCancellationRequested)
            {
                throw new ThreadInterruptedException();
            }
            var myTask = new MyTask<T>(task, this, _cancellationToken.Token);
            tasks.Enqueue(myTask.Run);
            return myTask;
        }
        
        /// <summary>
        /// заканчивает работу MyThreadPool
        /// </summary>
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
                    if (_cancellationToken.Token.IsCancellationRequested && tasks.IsEmpty)
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