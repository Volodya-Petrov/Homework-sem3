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
                Console.WriteLine("По заданному пути не обнаружено директории");
                return;
            }
            var myNUnit = new MyNUnit();
            var result = myNUnit.RunTests(args[0]);
            foreach (var info in result)
            {
                switch (info.State)
                {
                    case TestState.Success:
                        Console.WriteLine($"Тест {info.Name} класса {info.ClassName} прошел успешно\n Время выполнения теста: {info.Time} ms");
                        break;
                    case TestState.Failed:
                        Console.WriteLine($"Тест {info.Name} класса {info.ClassName} провален.\n Сообщение об ошибке: {info.ErrorMessage}");
                        break;
                    case TestState.Ignored:
                        Console.WriteLine($"Тест {info.Name} класса {info.ClassName} не был исполнен.\nПричина: {info.IgnoreMessage}");
                        break;
                    default:
                        Console.WriteLine($"При исполнение теста {info.Name} класса {info.ClassName} возникла ошибка.\nИнформация об ошибке:{info.ErrorMessage}");
                        break;
                }
            }
        }
    }
}