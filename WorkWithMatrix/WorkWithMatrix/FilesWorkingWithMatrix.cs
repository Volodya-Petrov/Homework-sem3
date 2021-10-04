using System;
using System.IO;

namespace WorkWithMatrix
{   
    /// <summary>
    /// Класс для записи и чтения матриц с файла
    /// </summary>
    public static class FilesWorkingWithMatrix
    {   
        /// <summary>
        /// считывает матрицу с файла
        /// </summary>
        public static int[][] ReadMatrixFromFile(string matrixPath)
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
        
        /// <summary>
        /// записывает матрицу в файл
        /// </summary>
        public static void WriteMatrixIntoFile(string filePath, int[][] matrix)
        {
            using var file = new StreamWriter(filePath, false);
            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix[i].Length; j++)
                {
                    file.Write(j != matrix[i].Length - 1 ? $"{matrix[i][j]} " : $"{matrix[i][j]}\n");
                }
            }
        }
    }
}