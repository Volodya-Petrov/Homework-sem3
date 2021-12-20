using System;

namespace AttributesForMyNUnit
{   
    /// <summary>
    /// Атрибут для NUnit тестов, устанавливается для методов, которые должны вызываться после каждого теста
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class After : Attribute
    {
        public After()
        {
            
        }
    }
}