using System;

namespace MyNUnit
{   
    /// <summary>
    /// Атрибут для NUnit тестов, устанавливается для методов, которые должны вызываться перед тестами
    /// </summary>
    public class BeforeClass : Attribute
    {
        public BeforeClass()
        {
            
        }
    }
}