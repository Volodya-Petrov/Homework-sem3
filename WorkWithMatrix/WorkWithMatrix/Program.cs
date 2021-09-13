using System;
using System.Diagnostics;
using System.IO;

namespace WorkWithMatrix
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите путь к первой матрице");
            var firstPath = Console.ReadLine();
            Console.WriteLine("Введите путь к второй матрице");
            var secondPath = Console.ReadLine();
            Console.WriteLine("Введите путь к файлу, куда записать произведение матриц");
            var thirdPath = Console.ReadLine();
            var stopwatch1 = new Stopwatch();
            stopwatch1.Start();
            ParallelMatrixMultiplication.MultiplyMatricesParallel(firstPath, secondPath, thirdPath);
            stopwatch1.Stop();
            var time1 = stopwatch1.Elapsed;
            Console.WriteLine($"Время с параллелькой: {time1}");
            var stopwatch2 = new Stopwatch();
            stopwatch2.Start();
            ParallelMatrixMultiplication.MultiplyMatricesNotParallel(firstPath, secondPath, thirdPath);
            stopwatch2.Stop();
            Console.WriteLine($"Время без параллельки: {stopwatch2.Elapsed}");
        }
    }
}