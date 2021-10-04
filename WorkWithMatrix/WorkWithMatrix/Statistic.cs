using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace WorkWithMatrix
{   
    /// <summary>
    /// класс для подсчета статистики многопоточного умножения матриц
    /// </summary>
    public static class Statistic
    {
        /* умножение матриц 500x500, кол-во экспериментов: 100
        /// Матожидание с мультипотоком: 257ms
        /// Среднеквадратичное отклонение с мультипотоком: 36ms
        /// Матожидание в однопоток: 1093ms
        /// Среднеквадратичное отклонение в однопоток: 167ms 

        
        /// Умножение матриц 250x250, кол-во экспериментов: 100
        /// Матожидание с мультипотоком: 36ms
        /// Среднеквадратичное отклонение с мультипотоком: 4ms
        /// Матожидание в однопоток: 105ms
        /// Среднеквадратичное отклонение в однопоток: 7ms 


        /// умножение матриц 100x100, кол-во экспериментов: 10
        /// Матожидание с мультипотоком: 3.2ms
        /// Среднеквадратичное отклонение с мультипотоком: 0.7ms
        /// Матожидание в однопоток: 5.9ms
        /// Среднеквадратичное отклонение в однопоток: 0.9ms
        */
        
        /// <summary>
        /// Генерирует матрицы, умножает их, считает матожидание и среднеквадратичное и выводит результаты в консоль
        /// </summary>
        /// <param name="rows">кол-во строк в сгенерированных матрицах</param>
        /// <param name="columns">кол-во столбцов в сгенерированных матрицах</param>
        /// <param name="countOfExperiments"></param>
        public static void CalculateAndPrintAverageAndStandardDeviationOfMultiplicationMatrices(int rows, int columns, int countOfExperiments)
        {
            (var parallelResults, var notParallelResults) = GenerateAndGetTimeResultsOfExperiments(rows, columns, countOfExperiments);

            (var averageParralel, var standardDeviationParrallel) =
                CalculateMathematicalExpectationAndDispersion(parallelResults);
            Console.WriteLine($"C многопоточностью:\nМатожидание = {averageParralel}\nСреднеквадратичное отклонение = {standardDeviationParrallel}");
            (var average, var standardDeviation) = CalculateMathematicalExpectationAndDispersion(notParallelResults);
            Console.WriteLine($"Без многопоточности:\n Матожидание = {average}\nСреднеквадратичное отклонение = {standardDeviation}");
        }
        
        private static (List<long>, List<long>) GenerateAndGetTimeResultsOfExperiments(int rows, int columns, int countOfExperiments)
        {
            var parallelResults = new List<long>();
            var notParallelResults = new List<long>();
            for (int i = 0; i < countOfExperiments; i++)
            {
                var firstMatrix = GenerateMatrix(rows, columns);
                var secondMatrix = GenerateMatrix(rows, columns);
                parallelResults.Add(CalculateTimeOfMultiplicationOfTwoMatrices(firstMatrix, secondMatrix,
                    ParallelMatrixMultiplication.MultiplyMatricesParallel));
                notParallelResults.Add(CalculateTimeOfMultiplicationOfTwoMatrices(firstMatrix, secondMatrix,
                    ParallelMatrixMultiplication.MultiplyMatricesNotParallel));
            }

            return (parallelResults, notParallelResults);
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