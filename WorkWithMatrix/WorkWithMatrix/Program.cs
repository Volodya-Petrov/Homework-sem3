using System;

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
            ParallelMatrixMultiplication.MultiplyMatricesParallel(firstPath, secondPath, thirdPath);
        }
    }
}