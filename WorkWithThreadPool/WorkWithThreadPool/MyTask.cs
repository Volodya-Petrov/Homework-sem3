using System;

namespace WorkWithThreadPool
{
    public class MyTask<T> : IMyTask<T>
    {
        private T _result;

        private bool _isCompleted;
        
        private Func<T> task;
        
        public MyTask(Func<T> task)
        {
            this.task = task;
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

        public void Run()
        {
            _result = task();
            _isCompleted = true;
        }
    }
}