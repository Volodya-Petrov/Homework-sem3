using System;

namespace WorkWithLazy
{
    public class Lazy<T> : ILazy<T>
    {
        public Lazy(Func<T> supplier)
        {
            _supplier = supplier;
        }
            
        private T _result;

        private bool _calculated;
        
        private readonly Func<T> _supplier;
        
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