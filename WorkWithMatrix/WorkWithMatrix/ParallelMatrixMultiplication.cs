using System;
using System.Threading;
using System.IO;

namespace WorkWithMatrix
{

    public static class ParallelMatrixMultiplication
    {   
               
        private static void CheckForMatrix(int[][] matrix)
        {
            var length = matrix[0].Length;
            foreach (var row in matrix)
            {
                if (row.Length != length)
                {
                    throw new ArgumentException();
                }
            }
        }
        
        private static int[][] ParseMatrixFromFile(string matrixPath)
        {
            var matrixInfoFromFile = File.ReadAllLines(matrixPath);
            var splitMatrix = new string[matrixInfoFromFile.Length][];
            for (int i = 0; i < matrixInfoFromFile.Length; i++)
            {
                splitMatrix[i] = matrixInfoFromFile[i].Split(" ");
            }

            var matrix = new int[splitMatrix.Length][];
            for (int i = 0; i < splitMatrix.Length; i++)
            {
                matrix[i] = new int[splitMatrix[i].Length];
                for (int j = 0; j < splitMatrix[i].Length; j++)
                {
                    int.TryParse(splitMatrix[i][j], out matrix[i][j]);
                }
            }

            CheckForMatrix(matrix);
            return matrix;
        }

        private static void WriteMatrixIntoFile(string filePath, int[][] matrix)
        {
            using var file = new StreamWriter(filePath, false);
            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix[i].Length; j++)
                {
                    file.Write(j != matrix[i].Length - 1 ? $"{matrix[i][j]} " : $"{matrix[i][j]}\n");
                }
            }
            file.Close();
        }
        
        private static void CheckForAbilityToMultiplyMatrices(int[][] firstMatrix, int[][] secondMatrix)
        {
            if (firstMatrix[0].Length != secondMatrix.Length)
            {
                throw new ArgumentException();
            }
        }
        
        public static void MultiplyMatricesNotParallel(string firstMatrixPath, string secondMatrixPath,
            string destinationPath)
        {
            var firstMatrix = ParseMatrixFromFile(firstMatrixPath);
            var secondMatrix = ParseMatrixFromFile(secondMatrixPath);
            CheckForAbilityToMultiplyMatrices(firstMatrix, secondMatrix);
            var newMatrix = new int[firstMatrix.Length][];
            for (int i = 0; i < firstMatrix.Length; i++)
            {
                newMatrix[i] = MultiplyRowOnMatrix(firstMatrix[i], secondMatrix);
            }
            WriteMatrixIntoFile(destinationPath, newMatrix);
        }
        
        public static void MultiplyMatricesParallel(string firstMatrixPath, string secondMatrixPath,
            string destinationPath)

        {
            var firstMatrix = ParseMatrixFromFile(firstMatrixPath);
            var secondMatrix = ParseMatrixFromFile(secondMatrixPath);
            CheckForAbilityToMultiplyMatrices(firstMatrix, secondMatrix);
            var newMatrix = new int[firstMatrix.Length][];
            var threads = new Thread[firstMatrix.Length];
            for (int i = 0; i < firstMatrix.Length; i++)
            {
                var index = i;
                threads[i] = new Thread(() => { newMatrix[index] = MultiplyRowOnMatrix(firstMatrix[index], secondMatrix); });
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }
            WriteMatrixIntoFile(destinationPath, newMatrix);
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