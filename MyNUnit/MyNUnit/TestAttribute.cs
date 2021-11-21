using System;

namespace MyNUnit
{
    public class TestAttribute : Attribute
    {
        public TestAttribute(Type expected, string ignore)
        {
            Expected = expected;
            Ignore = ignore;
        }
        
        public Type Expected { get; }
        
        public string Ignore { get; }
    }
}