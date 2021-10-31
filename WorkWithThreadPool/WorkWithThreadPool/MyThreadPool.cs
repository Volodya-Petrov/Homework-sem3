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

        private AutoResetEvent taskSubmitted;

        private CancellationTokenSource _cancellationToken;
        
        
        /// <summary>
        /// Класс для параллельных вычислений
        /// </summary>
        private class MyTask<T> : IMyTask<T>
        {
            private T _result;
    
            private MyThreadPool _threadPool;
    
            private bool _isCompleted;
            
            private Func<T> task;
    
            private ConcurrentQueue<Action> _continueTasks;

            private AggregateException _exception;

            private ManualResetEvent resultCalculated;
            
            public MyTask(Func<T> task, MyThreadPool pool)
            {
                this.task = task;
                _threadPool = pool;
                _continueTasks = new ConcurrentQueue<Action>();
                resultCalculated = new ManualResetEvent(false);
            }
            
            /// <inheritdoc />
            public T Result
            {
                get
                {
                    resultCalculated.WaitOne();
                    if (_exception != null)
                    {
                        throw _exception;
                    }
                    return _result;
                }
            }
            
            /// <inheritdoc />
            public bool IsCompleted { get => _isCompleted; }
            
            /// <summary>
            /// функция вычисления результата задачи
            /// </summary>
            public void Run()
            {
                try
                {
                    _result = task();
                }
                catch (Exception e)
                {
                    _exception = new AggregateException(e);
                }
                _isCompleted = true;
                resultCalculated.Set();
                while (!_continueTasks.IsEmpty)
                {
                    if (_continueTasks.TryDequeue(out Action continueTask))
                    {
                        _threadPool.tasks.Enqueue(continueTask);
                        _threadPool.taskSubmitted.Set();
                    }
                }
            }
            
            /// <inheritdoc />
            public IMyTask<TResult> ContinueWith<TResult>(Func<T, TResult> continueTask)
            {
                lock (_threadPool._cancellationToken)
                {
                    if (_threadPool._cancellationToken.Token.IsCancellationRequested)
                    {
                        throw new ThreadInterruptedException();
                    }
                    if (_isCompleted)
                    {
                        return _threadPool.Submit(() =>
                        {
                            if (_exception != null)
                            {
                                throw _exception;
                            }
                            return continueTask(_result);
                        });
                    }
                    var newContinueTask = new MyTask<TResult>(() =>
                    {
                        if (_exception != null)
                        {
                            throw _exception;
                        }
                        return continueTask(_result);
                    }, _threadPool);
                    _continueTasks.Enqueue(newContinueTask.Run);
                    return newContinueTask;
                }
            }
        }
        
        public MyThreadPool(int countOfThreads)
        {
            _cancellationToken = new CancellationTokenSource();
            tasks = new ConcurrentQueue<Action>();
            threads = new Thread[countOfThreads];
            taskSubmitted = new AutoResetEvent(false);
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
            lock (_cancellationToken)
            {
                if (_cancellationToken.Token.IsCancellationRequested)
                {
                    throw new ThreadInterruptedException();
                }
                var myTask = new MyTask<T>(task, this);
                tasks.Enqueue(myTask.Run);
                taskSubmitted.Set();
                return myTask;
            }
        }
        
        /// <summary>
        /// заканчивает работу MyThreadPool
        /// </summary>
        public void Shutdown()
        {
            lock (_cancellationToken)
            {
                _cancellationToken.Cancel();
                
            }

            taskSubmitted.Set();
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
                    if (_cancellationToken.Token.IsCancellationRequested && tasks.IsEmpty)
                    {
                        return;
                    }
                    taskSubmitted.WaitOne();
                    if (!tasks.IsEmpty)
                    {
                        taskSubmitted.Set();
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