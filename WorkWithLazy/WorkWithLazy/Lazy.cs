using System;

namespace WorkWithLazy
{   
    /// <summary>
    /// Класс для вычисления значения переданной функции в однопотоке
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Lazy<T> : ILazy<T>
    {
        public Lazy(Func<T> supplier)
        {
            if (supplier == null)
            {
                throw new ArgumentException();
            }
            _supplier = supplier;
        }
            
        private T _result;

        private bool _calculated;
        
        private readonly Func<T> _supplier;
        
        /// <summary>
        /// вычисляет значение переданной функции
        /// </summary>
        /// <returns></returns>
        public T Get()
        {
            if (_calculated)
            {
                return _result;
            }
            _result = _supplier();
            _calculated = true;
            return _result;
        }
    }
}