using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace WorkWithMatrix
{
    public static class Statistic
    {   
        /// <summary>
        /// Матожидание с мультипотоком: 3200ms
        /// Среднеквадратичное отклонение с мультипотоком: 748ms
        /// Матожидание в однопоток: 5900ms
        /// Среднеквадратичное отклонение в однопоток: 943ms 
        /// </summary>
        public static void GetStatisticOfMatrices100X100And10Experiments()
        {
            var parallelResults = new List<long>();
            var notParallelResults = new List<long>();
            for (int i = 0; i < 10; i++)
            {
                var firstMatrix = GenerateMatrix(100, 100);
                var secondMatrix = GenerateMatrix(100, 100);
                parallelResults.Add(CalculateTimeOfMultiplicationOfTwoMatrices(firstMatrix, secondMatrix,
                    ParallelMatrixMultiplication.MultiplyMatricesParallel));
                notParallelResults.Add(CalculateTimeOfMultiplicationOfTwoMatrices(firstMatrix, secondMatrix,
                    ParallelMatrixMultiplication.MultiplyMatricesNotParallel));
            }

            (var averageParralel, var standardDeviationParrallel) =
                CalculateMathematicalExpectationAndDispersion(parallelResults);
            Console.WriteLine($"Матожидание = {averageParralel}\nСреднеквадратичное отклонение = {standardDeviationParrallel}");
            (var average, var standardDeviation) = CalculateMathematicalExpectationAndDispersion(notParallelResults);
            Console.WriteLine($"Матожидание = {average}\nСреднеквадратичное отклонение = {standardDeviation}");
        }

        private static (double average, double standardDeviation) CalculateMathematicalExpectationAndDispersion(List<long> resultOfExperiment)
        {
            var average = resultOfExperiment.Average();
            var dispersion = resultOfExperiment.Select(x => Math.Pow(x - average, 2)).Average();
            var standardDeviation = Math.Sqrt(dispersion);
            return (average, standardDeviation);
        }
        
        private static long CalculateTimeOfMultiplicationOfTwoMatrices(int[][] firstMatrix, int[][] secondMatrix,
            Func<int[][], int[][], int[][]> multiply)
        {
            var timer = new Stopwatch();
            timer.Start();
            multiply(firstMatrix, secondMatrix);
            timer.Stop();
            return timer.ElapsedMilliseconds;
        }
        
        private static int[][] GenerateMatrix(int rowsCount, int columnsCount)
        {
            var matrix = new int[rowsCount][];
            var random = new Random();
            for (int i = 0; i < rowsCount; i++)
            {
                var row = new int[columnsCount];
                for (int j = 0; j < row.Length; j++)
                {
                    row[j] = random.Next(0, 100);
                }

                matrix[i] = row;
            }
            return matrix;
        }
    }
}