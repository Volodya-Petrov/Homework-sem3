using System;
using System.Collections.Concurrent;
using System.Threading;

namespace WorkWithThreadPool
{
    public class MyTask<T> : IMyTask<T>
    {
        private T _result;

        private MyThreadPool _threadPool;

        private bool _isCompleted;
        
        private Func<T> task;

        private ConcurrentQueue<Action> _continueTasks;
        
        private CancellationToken _cancellationToken;
        
        public MyTask(Func<T> task, MyThreadPool pool, CancellationToken token)
        {
            _cancellationToken = token;
            this.task = task;
            _threadPool = pool;
            _continueTasks = new ConcurrentQueue<Action>();
        }
        
        public T Result
        {
            get
            {
                while (!_isCompleted);
                return _result;
            }
        }

        public bool IsCompleted { get => _isCompleted; }
        
        /// <summary>
        /// функция вычисления результата задачи
        /// </summary>
        public void Run()
        {
            _result = task();
            _isCompleted = true;
            while (!_continueTasks.IsEmpty)
            {
                if (_continueTasks.TryDequeue(out Action continueTask))
                {
                    continueTask();
                }
            }
        }

        public IMyTask<TResult> ContinueWith<TResult>(Func<T, TResult> continueTask)
        {
            if (_cancellationToken.IsCancellationRequested)
            {
                throw new ThreadInterruptedException();
            }
            if (_isCompleted)
            {
                return _threadPool.Submit(() => continueTask(_result));
            }
            var newContinueTask = new MyTask<TResult>(() => continueTask(_result), _threadPool, _cancellationToken);
            _continueTasks.Enqueue(newContinueTask.Run);
            return newContinueTask;
        }
    }
}