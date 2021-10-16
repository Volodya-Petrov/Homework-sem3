using System;

namespace WorkWithThreadPool
{
    public interface IMyTask<TResult>
    {
        public bool IsCompleted { get; }
        
        public TResult Result { get; }

        //public TNewResult ContinueWith<TNewResult>(Func<TResult, TNewResult> newTask);
    }
}