using System;

namespace WorkWithLazy
{
    public static class LazyFactory<T>
    {
        public static CoolerLazy<T> CreateCoolerLazy(Func<T> supplier)
            => new CoolerLazy<T>(supplier);
        
        public static Lazy<T> CreateLazy(Func<T> supplier)
            => new Lazy<T>(supplier);
    }
}