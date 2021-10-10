using System;

namespace WorkWithLazy
{   
    /// <summary>
    /// Класс для вычисления значения переданной функции
    /// </summary>
    public class Lazy<T> : ILazy<T>
    {
        private T _result;

        private bool _calculated;
        
        private Func<T> _supplier;
        
        public Lazy(Func<T> supplier)
        {
            if (supplier == null)
            {
                throw new ArgumentNullException();
            }
            _supplier = supplier;
        }

        /// <inheritdoc />
        public T Get()
        {
            if (_calculated)
            {
                return _result;
            }
            _result = _supplier();
            _supplier = null;
            _calculated = true;
            return _result;
        }
    }
}