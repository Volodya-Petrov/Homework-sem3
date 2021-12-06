using System;

namespace MyNUnit
{   
    /// <summary>
    /// Атрибут для NUnit тестов, помечает методы, которые являются тестовыми
    /// </summary>
    public class Test : Attribute
    {
        public Test(Type expected, string ignore = null)
        {
            Expected = expected;
            Ignore = ignore;
        }
        
        public Type Expected { get; }
        
        public string Ignore { get; }
    }
}