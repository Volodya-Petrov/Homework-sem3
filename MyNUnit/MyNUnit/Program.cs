using System;

namespace MyNUnit
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите путь до директории с тестами:");
            var path = Console.ReadLine();
            var myNUnit = new MyNUnit();
            var result = myNUnit.RunTests(path);
            foreach (var info in result)
            {
                Console.WriteLine(info);
            }
        }
    }
}