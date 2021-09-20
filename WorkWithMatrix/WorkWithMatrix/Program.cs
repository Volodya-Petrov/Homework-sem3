using System;
using System.Diagnostics;
using System.IO;

namespace WorkWithMatrix
{
    class Program
    {
        static void Main(string[] args)
        {
            var firstMatrix = FilesWorkingWithMatrix.ReadMatrixFromFile(args[0]);
            var secondMatrix = FilesWorkingWithMatrix.ReadMatrixFromFile(args[1]);
            var resultMatrixWithParralel =
                ParallelMatrixMultiplication.MultiplyMatricesParallel(firstMatrix, secondMatrix);
            FilesWorkingWithMatrix.WriteMatrixIntoFile(args[2], resultMatrixWithParralel);
        }
    }
}