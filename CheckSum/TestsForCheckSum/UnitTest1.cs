using System.Security.Cryptography;
using NUnit.Framework;
using CheckSum;

namespace TestProject1
{
    public class Tests
    {
        private string path1 = "../../../../../Test1.1/";
        private string path2 = "../../../../Test";
        private CheckSum.CheckSum checkSum;
        [SetUp]
        public void Setup()
        {
            checkSum = new();
        }

        [Test]
        public void TestHashDoesntChange()
        {
            var result1 = checkSum.Calculate(path1);
            var result2 = checkSum.Calculate(path1);
            var result3 = checkSum.Calculate(path1);
            Assert.AreEqual(result1, result2);
            Assert.AreEqual(result1, result3);
        }

        [Test]
        public void CheckForDirectoryDidntChange()
        {
            var expected = new byte[] {26, 134, 168, 249, 151, 244, 104, 225};
            var result = checkSum.Calculate(path2);
            Assert.AreEqual(expected, result);
        }
    }
}