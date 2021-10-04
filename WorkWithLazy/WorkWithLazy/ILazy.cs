using System;

namespace WorkWithLazy
{   
    /// <summary>
    /// интерфейс для классов, выполняющих ленивое вычисление
    /// </summary>
    public interface ILazy<out T>
    {
        /// <summary>
        /// выдает результат вычисления
        /// </summary>
        public T Get();
    }
}