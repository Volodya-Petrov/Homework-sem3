using System;

namespace WorkWithLazy
{
    public interface ILazy<T>
    {
        public T Get();
    }
}