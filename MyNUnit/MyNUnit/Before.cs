using System;

namespace MyNUnit
{
    /// <summary>
    /// Атрибут для NUnit тестов, устанавливается для методов, которые должны вызываться перед каждым тестом
    /// </summary>
    public class Before : Attribute
    {
        public Before()
        {
            
        }
    }
}