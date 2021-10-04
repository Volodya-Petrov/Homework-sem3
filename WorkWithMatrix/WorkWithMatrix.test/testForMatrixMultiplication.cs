using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace WorkWithMatrix.test
{
    public class Tests
    {

        [TestCaseSource(nameof(FunctionsForTest))]
        public void TestForCalculationSquareMatricesWithFilesWorking(Func<int[][], int[][], int[][]> multiplyMatrices)
        {
            var path1 = "../../../test1/matrix1.txt";
            var path2 = "../../../test1/matrix2.txt";
            var path3 = "../../../test1/matrix3.txt";
            var firstMatrix = FilesWorkingWithMatrix.ReadMatrixFromFile(path1);
            var secondMatrix = FilesWorkingWithMatrix.ReadMatrixFromFile(path2);
            var resultMatrix = multiplyMatrices(firstMatrix, secondMatrix);
            FilesWorkingWithMatrix.WriteMatrixIntoFile(path3, resultMatrix);
            var expected = "30 36 42\n66 81 96\n102 126 150\n";
            Assert.AreEqual(expected, File.ReadAllText(path3));
        }
        
        [TestCaseSource(nameof(FunctionsForTest))]
        public void TestForCalculationNotSquareMatrices(Func<int[][], int[][], int[][]> multiplyMatrices)
        {
            var firstMatrix = new int[][]
            {
                new[] {1, 2, 3, 4},
                new[] {5, 6, 7, 8},
                new[] {9, 10, 11, 12}
            };
            var secondMatrix = new int[][]
            {
                new[] {1, 2, 3},
                new[] {4, 5, 6},
                new[] {7, 8, 9},
                new[] {10, 11, 12}
            };
            var resultMatrix = multiplyMatrices(firstMatrix, secondMatrix);
            var expected = new int[][]
            {
                new[] {70, 80, 90},
                new[] {158, 184, 210},
                new[] {246, 288, 330}
            };
            Assert.AreEqual(expected, resultMatrix);
        }

        [TestCaseSource(nameof(FunctionsForTest))]
        public void TestReadIncorrectMatrixShouldThrowException(Func<int[][], int[][], int[][]> multiplyMatrices)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var path1 = "../../../test3/matrix1.txt";
                var firstMatrix = FilesWorkingWithMatrix.ReadMatrixFromFile(path1);
            });
        }
        
        [TestCaseSource(nameof(FunctionsForTest))]
        public void TestWithMatricesYouCanNotMultiplyShouldThrowException(Func<int[][], int[][], int[][]> multiplyMatrices)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var firstMatrix = new int[][]
                {
                    new[] {1, 2, 3},
                    new[] {4, 5, 6},
                    new[] {7, 8, 9}
                };
                var secondMatrix = new int[][]
                {
                    new[] {1, 2, 3},
                    new[] {1, 2, 3}
                };
                var resultMatrix = multiplyMatrices(firstMatrix, secondMatrix);
            });
        }
        
        private static IEnumerable<Func<int[][], int[][], int[][]>> FunctionsForTest()
        {
            yield return ParallelMatrixMultiplication.MultiplyMatricesParallel;
            yield return ParallelMatrixMultiplication.MultiplyMatricesNotParallel;
        }
    }
}