using System;

namespace WorkWithLazy
{   
    /// <summary>
    /// Класс вычисляет значение переданной функции, работает с многопоточностью
    /// </summary>
    public class CoolerLazy<T> : ILazy<T>
    {   
        private T _result;

        private volatile bool _calculated;
        
        private Func<T> _supplier;

        private object locker = new();
        
        public CoolerLazy(Func<T> supplier)
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
            lock (locker)
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
}