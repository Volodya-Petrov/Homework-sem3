using System;

namespace AttributesForMyNUnit
{   
    /// <summary>
    /// Атрибут для NUnit тестов, устанавливается для методов, которые должны вызываться перед тестами
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)] 
    public class BeforeClass : Attribute
    {
        public BeforeClass()
        {
            
        }
    }
}