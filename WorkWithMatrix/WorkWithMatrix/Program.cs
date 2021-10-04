using System;
using System.Diagnostics;
using System.IO;

namespace WorkWithMatrix
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Введены не все аргументы");
                return;
            }

            if (!File.Exists(args[0]) || !File.Exists(args[1]) || !File.Exists(args[2]))
            {
                Console.WriteLine("Какой-то из путей ведет в несуществующий файл");
                return;
            }
            var firstMatrix = FilesWorkingWithMatrix.ReadMatrixFromFile(args[0]);
            var secondMatrix = FilesWorkingWithMatrix.ReadMatrixFromFile(args[1]);
            var resultMatrixWithParralel =
                ParallelMatrixMultiplication.MultiplyMatricesParallel(firstMatrix, secondMatrix);
            FilesWorkingWithMatrix.WriteMatrixIntoFile(args[2], resultMatrixWithParralel);
        }
    }
}