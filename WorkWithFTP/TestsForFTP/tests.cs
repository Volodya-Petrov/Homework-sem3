using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorkWithFTPClient;
using WorkWithFTP;
using NUnit.Framework;

namespace TestsForFTP
{
    public class Tests
    {
        private Client client = new Client("127.0.0.1", 1488);
        private Server server = new Server("127.0.0.1", 1488) ;
        private string pathForData = "../../../data/";

        [Test]
        public async Task TestShouldThrowExceptionIfListWithIncorrectPath()
        {
            server.StartServer();
            Assert.Throws<AggregateException>(() => client.List("pluuuuug").Wait());
            server.StopServer();
        }
        
        [Test]
        public async Task TestShouldThrowExceptionIfGetWithIncorrectPath()
        {
            server.StartServer();
            Assert.Throws<AggregateException>(() => client.Get("pluuuuug").Wait());
            server.StopServer();
        }
        
        [Test]
        public async Task TestForList()
        {
            server.StartServer();
            var result = new[]
            {
                (pathForData + "plug.txt", false),
                (pathForData + "WorkWithVK.dll", false),
                (pathForData + "kek", true),
                (pathForData + "lol", true)
            };
            var response = await client.List(pathForData);
            server.StopServer();
            Assert.AreEqual(result.Length, response.Length);
            for (int i = 0; i < result.Length; i++)
            {
                Assert.IsTrue(response.Any(x => x == result[i]));
            }
        }

        [Test]
        public async Task TestForGet()
        {
            server.StartServer();
            var pathForFile = pathForData + "WorkWithVK.dll";
            var result = File.ReadAllBytes(pathForFile);
            var response = await client.Get(pathForFile);
            server.StopServer();
            Assert.AreEqual(result.Length, response.Item1);
        }
    }
}