using System;

namespace WorkWithLazy
{
    public class CoolerLazy<T> : ILazy<T>
    {
        public CoolerLazy(Func<T> supplier)
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
            lock (_result)
            {
                _result = _supplier();
                _calculated = true;
                return _result;   
            }
        }
    }
}