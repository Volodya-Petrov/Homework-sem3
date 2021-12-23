using System;

namespace AttributesForMyNUnit
{
    /// <summary>
    /// Атрибут для NUnit тестов, устанавливается для методов, которые должны вызываться перед каждым тестом
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class Before : Attribute
    {
        public Before()
        {
            
        }
    }
}