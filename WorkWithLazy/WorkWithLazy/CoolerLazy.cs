using System;

namespace WorkWithLazy
{   
    /// <summary>
    /// Класс вычисляет значение переданной функции, работает с мультипотоком
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CoolerLazy<T> : ILazy<T>
    {
        public CoolerLazy(Func<T> supplier)
        {
            if (supplier == null)
            {
                throw new ArgumentException();
            }
            _supplier = supplier;
        }
            
        private T _result;

        private volatile bool _calculated;
        
        private readonly Func<T> _supplier;

        private object locker = new object();
        
        /// <summary>
        /// метод вычисляющий значения переданной функции
        /// </summary>
        /// <returns></returns>
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
                _calculated = true;
                return _result;   
            }
        }
    }
}