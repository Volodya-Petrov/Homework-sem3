using System.IO;

namespace WorkWithMatrix
{
    public static class ParallelMatrixMultiplication
    {
        public static int[][] ParseMatrixFromFile(string matrixPath)
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
                for (int j = 0; j < splitMatrix[0].Length; j++)
                {
                    int.TryParse(splitMatrix[i][j], out matrix[i][j]);
                }
            }

            return matrix;
        }
        
        public static void CalculateMatrixMultiplication(string firstMatrixPath, string secondMatrixPath)
        {
            
        }
    }
}