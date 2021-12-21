using System;
using System.IO;

namespace MyNUnit
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Не был передан путь к тестам");
                return;
            }
            if (!Directory.Exists(args[0]))
            {
                Console.WriteLine("По указанному пути не существует директории");
                return;
            }
            var myNUnit = new MyNUnit();
            var result = myNUnit.RunTests(args[0]);
            foreach (var info in result)
            {
                Console.WriteLine(info);
            }
        }
    }
}