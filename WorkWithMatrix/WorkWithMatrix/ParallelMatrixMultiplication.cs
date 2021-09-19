using System;
using System.Threading;

namespace WorkWithMatrix
{   
    /// <summary>
    /// класс для умножения матриц
    /// </summary>
    public static class ParallelMatrixMultiplication
    {
        private static void CheckForAbilityToMultiplyMatrices(int[][] firstMatrix, int[][] secondMatrix)
        {
            if (firstMatrix[0].Length != secondMatrix.Length)
            {
                throw new ArgumentException();
            }
        }
        
        /// <summary>
        /// Умножение матриц изпользуя 1 поток
        /// </summary>
        public static int[][] MultiplyMatricesNotParallel(int[][] firstMatrix, int[][] secondMatrix)
        {
            CheckForAbilityToMultiplyMatrices(firstMatrix, secondMatrix);
            var newMatrix = new int[firstMatrix.Length][];
            for (int i = 0; i < firstMatrix.Length; i++)
            {
                newMatrix[i] = MultiplyRowOnMatrix(firstMatrix[i], secondMatrix);
            }

            return newMatrix;
        }
        
        /// <summary>
        /// Умножение матриц используя многопоточность
        /// </summary>
        public static int[][] MultiplyMatricesParallel(int[][] firstMatrix, int[][] secondMatrix)
        {
            CheckForAbilityToMultiplyMatrices(firstMatrix, secondMatrix);
            var newMatrix = new int[firstMatrix.Length][];
            var threads = new Thread[Environment.ProcessorCount];
            var chunkSize = newMatrix.Length / threads.Length + 1;
            for (int i = 0; i < threads.Length; i++)
            {
                var index = i;
                threads[i] = new Thread(() =>
                {
                    for (var j = index * chunkSize; j < chunkSize * (index + 1) && j < newMatrix.Length; j++)
                    {
                        newMatrix[j] = MultiplyRowOnMatrix(firstMatrix[j], secondMatrix);
                    }
                });
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            return newMatrix;
        }

        private static int[] MultiplyRowOnMatrix(int[] row, int[][] matrix)
        {
            var newRow = new int[matrix[0].Length];
            for (int i = 0; i < matrix[0].Length; i++)
            {
                var sum = 0;
                for (int j = 0; j < matrix.Length; j++)
                {
                    sum += row[j] * matrix[j][i];
                }

                newRow[i] = sum;
            }

            return newRow;
        }
    }
}