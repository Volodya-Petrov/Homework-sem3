using System;

namespace WorkWithThreadPool
{   
    /// <summary>
    /// Интерфейс для объектов, возвращаемых MyThreadPool, нужного для получения результата вычисления задачи
    /// </summary>
    public interface IMyTask<TResult>
    {   
        /// <summary>
        /// проверяет, досчитана ли отправленная задача
        /// </summary>
        public bool IsCompleted { get; }
        
        /// <summary>
        /// выдает результат вычисления задачи
        /// </summary>
        public TResult Result { get; }
        
        /// <summary>
        /// добавление новой задачи, которая зависит от результата исходной задачи
        /// </summary>
        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> newTask);
    }
}