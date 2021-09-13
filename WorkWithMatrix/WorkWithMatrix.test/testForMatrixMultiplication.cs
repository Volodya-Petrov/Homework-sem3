using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace WorkWithMatrix.test
{
    public class Tests
    {

        [TestCaseSource(nameof(FunctionsForTest))]
        public void TestForCalculationSquareMatrices(Action<string, string, string> multiplyMatrices)
        {
            var path1 = "../../../test1/matrix1.txt";
            var path2 = "../../../test1/matrix2.txt";
            var path3 = "../../../test1/matrix3.txt";
            multiplyMatrices(path1, path2, path3);
            var expected = "30 36 42\n66 81 96\n102 126 150\n";
            Assert.AreEqual(expected, File.ReadAllText(path3));
        }
        
        [TestCaseSource(nameof(FunctionsForTest))]
        public void TestForCalculationNotSquareMatrices(Action<string, string, string> multiplyMatrices)
        {
            var path1 = "../../../test2/matrix1.txt";
            var path2 = "../../../test2/matrix2.txt";
            var path3 = "../../../test2/matrix3.txt";
            multiplyMatrices(path1, path2, path3);
            var expected = "70 80 90\n158 184 210\n246 288 330\n";
            Assert.AreEqual(expected, File.ReadAllText(path3));
        }

        [TestCaseSource(nameof(FunctionsForTest))]
        public void TestWithIncorrectMatrixShouldThrowException(Action<string, string, string> multiplyMatrices)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var path1 = "../../../test3/matrix1.txt";
                var path2 = "../../../test3/matrix2.txt";
                var path3 = "../../../test3/matrix3.txt";
                multiplyMatrices(path1, path2, path3);
            });
        }
        
        [TestCaseSource(nameof(FunctionsForTest))]
        public void TestWithMatricesYouCanNotMultiplyShouldThrowException(Action<string, string, string> multiplyMatrices)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var path1 = "../../../test4/matrix1.txt";
                var path2 = "../../../test4/matrix2.txt";
                var path3 = "../../../test4/matrix3.txt";
                multiplyMatrices(path1, path2, path3);
            });
        }
        
        private static IEnumerable<Action<string, string, string>> FunctionsForTest()
        {
            yield return ParallelMatrixMultiplication.MultiplyMatricesParallel;
            yield return ParallelMatrixMultiplication.MultiplyMatricesNotParallel;
        }
    }
}