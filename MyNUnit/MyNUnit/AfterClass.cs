using System;

namespace MyNUnit
{
    /// <summary>
    /// Атрибут для NUnit тестов, устанавливается для методов, которые должны вызываться после тестов
    /// </summary>
    public class AfterClass : Attribute
    {
        public AfterClass()
        {
            
        }
    }
}