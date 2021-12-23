using System;

namespace AttributesForMyNUnit
{
    /// <summary>
    /// Атрибут для NUnit тестов, устанавливается для методов, которые должны вызываться после тестов
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AfterClass : Attribute
    {
        public AfterClass()
        {
            
        }
    }
}