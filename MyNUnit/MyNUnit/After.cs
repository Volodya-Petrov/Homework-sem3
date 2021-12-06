using System;

namespace MyNUnit
{   
    /// <summary>
    /// Атрибут для NUnit тестов, устанавливается для методов, которые должны вызываться после каждого теста
    /// </summary>
    public class After : Attribute
    {
        public After()
        {
            
        }
    }
}