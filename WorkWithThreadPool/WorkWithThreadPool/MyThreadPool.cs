using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WorkWithThreadPool
{   
    /// <summary>
    /// класс для параллельных исполнений задач
    /// </summary>
    public class MyThreadPool
    {
        private ConcurrentQueue<Action> tasks;

        private Thread[] threads;

        private AutoResetEvent threadPoolEvent;

        private CancellationTokenSource cancellationToken;

        private int tasksCount; 
        
        /// <summary>
        /// Класс для параллельных вычислений
        /// </summary>
        private class MyTask<T> : IMyTask<T>
        {
            private T result;
    
            private MyThreadPool threadPool;
    
            private bool isCompleted;
            
            private Func<T> task;
    
            private ConcurrentQueue<Action> continueTasks;

            private AggregateException exception;

            private ManualResetEvent resultCalculated;

            private object locker = new();

            public MyTask(Func<T> task, MyThreadPool pool)
            {
                this.task = task;
                threadPool = pool;
                continueTasks = new ConcurrentQueue<Action>();
                resultCalculated = new ManualResetEvent(false);
            }
            
            /// <inheritdoc />
            public T Result
            {
                get
                {
                    resultCalculated.WaitOne();
                    if (exception != null)
                    {
                        throw exception;
                    }
                    return result;
                }
            }
            
            /// <inheritdoc />
            public bool IsCompleted => isCompleted;
            
            /// <summary>
            /// функция вычисления результата задачи
            /// </summary>
            public void Run()
            {
                try
                {
                    result = task();
                }
                catch (Exception e)
                {
                    exception = new AggregateException(e);
                }

                task = null;
                isCompleted = true;
                resultCalculated.Set();
                lock (locker)
                {
                    while (!continueTasks.IsEmpty)
                    {
                        if (continueTasks.TryDequeue(out Action continueTask))
                        {
                            threadPool.SubmitAfterShutDown(continueTask);
                        }
                    }
                }
            }
            
            /// <inheritdoc />
            public IMyTask<TResult> ContinueWith<TResult>(Func<T, TResult> continueTask)
            {
                lock (threadPool.cancellationToken)
                {
                    if (threadPool.cancellationToken.Token.IsCancellationRequested)
                    {
                        throw new InvalidOperationException();
                    }

                    Func<TResult> continueTaskForThreadPool = () =>
                    {
                        if (exception != null)
                        {
                            throw exception;
                        }

                        return continueTask(result);
                    };
                    
                    lock (locker)
                    {
                        if (isCompleted)
                        {
                            return threadPool.Submit(continueTaskForThreadPool);
                        }
                        var newContinueTask = new MyTask<TResult>(continueTaskForThreadPool, threadPool);
                        threadPool.TaskAdded();
                        continueTasks.Enqueue(newContinueTask.Run);
                        return newContinueTask;
                    }
                }
            }
        }
        
        public MyThreadPool(int countOfThreads)
        {
            cancellationToken = new CancellationTokenSource();
            tasks = new ConcurrentQueue<Action>();
            threads = new Thread[countOfThreads];
            threadPoolEvent = new AutoResetEvent(false);
            for (int i = 0; i < countOfThreads; i++)
            {
                threads[i] = CreateThread();
                threads[i].Start();
            }
        }
        
        /// <summary>
        /// добавляет задачу в очередь на исполнение
        /// </summary>
        public IMyTask<T> Submit<T>(Func<T> task)
        {
            lock (cancellationToken)
            {
                if (cancellationToken.Token.IsCancellationRequested)
                {
                    throw new InvalidOperationException();
                }
                var myTask = new MyTask<T>(task, this);
                tasks.Enqueue(myTask.Run);
                Interlocked.Increment(ref tasksCount);
                threadPoolEvent.Set();
                return myTask;
            }
        }
        
        /// <summary>
        /// заканчивает работу MyThreadPool
        /// </summary>
        public void Shutdown()
        {
            lock (cancellationToken)
            {
                cancellationToken.Cancel();
            }

            threadPoolEvent.Set();
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Join();
            }
        }
        
        private Thread CreateThread()
        {
            var thread = new Thread(() =>
            {
                while (true)
                {
                    if (cancellationToken.Token.IsCancellationRequested && tasksCount == 0)
                    {
                        return;
                    }
                    threadPoolEvent.WaitOne();
                    if (!tasks.IsEmpty || cancellationToken.Token.IsCancellationRequested)
                    {
                        threadPoolEvent.Set();
                    }
                    if (tasks.TryDequeue(out Action task))
                    {
                        Interlocked.Decrement(ref tasksCount);
                        task();
                    }
                }
            });
            thread.IsBackground = true;
            return thread;
        }

        private void TaskAdded()
        {
            Interlocked.Increment(ref tasksCount);
        }

        private void SubmitAfterShutDown(Action task)
        {
            tasks.Enqueue(task);
            threadPoolEvent.Set();
        }
    }
}