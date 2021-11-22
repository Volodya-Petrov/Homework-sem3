using System;

namespace MyNUnit
{
    public class Test : Attribute
    {
        public Test(Type expected, string ignore)
        {
            Expected = expected;
            Ignore = ignore;
        }
        
        public Type Expected { get; }
        
        public string Ignore { get; }
    }
}