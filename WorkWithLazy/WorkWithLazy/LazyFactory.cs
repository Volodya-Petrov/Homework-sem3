using System;

namespace WorkWithLazy
{   
    /// <summary>
    /// Класс для создания Lazy объектов
    /// </summary>
    public static class LazyFactory<T>
    {   
        /// <summary>
        /// создает Lazy работающий в многопотоке
        /// </summary>
        public static CoolerLazy<T> CreateCoolerLazy(Func<T> supplier)
            => new CoolerLazy<T>(supplier);
        
        /// <summary>
        /// создает Lazy работающий в однопотоке
        /// </summary>
        public static Lazy<T> CreateLazy(Func<T> supplier)
            => new Lazy<T>(supplier);
    }
}