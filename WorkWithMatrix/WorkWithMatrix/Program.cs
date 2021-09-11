using System;

namespace WorkWithMatrix
{
    class Program
    {
        static void Main(string[] args)
        {
            var matrix = WorkWithMatrix.ParallelMatrixMultiplication.ParseMatrixFromFile("matrix1.txt");
        }
    }
}