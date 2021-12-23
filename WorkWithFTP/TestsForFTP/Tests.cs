using System;
using System.Globalization;
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
        private string ip = "127.0.0.1";
        private int port = 1488;
        private Client client;
        private Server server;
        private string pathForData = "../../../data/";
        private CancellationTokenSource tokenSource;

        [SetUp]
        public void SetUp()
        {
            server = new Server(ip, port);
            client = new Client(ip, port);
            tokenSource = new CancellationTokenSource();
        }
        
        [Test]
        public async Task TestShouldThrowExceptionIfListWithIncorrectPath()
        {
            using var handle = server.StartServer();
            Assert.Throws<AggregateException>(() => client.List("pluuuuug", tokenSource.Token).Wait());
        }
        
        [Test]
        public async Task TestShouldThrowExceptionIfGetWithIncorrectPath()
        {
            using var handle = server.StartServer();
            Assert.Throws<AggregateException>(() => client.Get("pluuuuug", null, tokenSource.Token).Wait());
        }
        
        [Test]
        public async Task TestForList()
        {
            using var handle = server.StartServer();
            var result = new[]
            {
                (pathForData + "plug.txt", false),
                (pathForData + "WorkWithVK.dll", false),
            };
            var response = await client.List(pathForData, tokenSource.Token);
            Assert.AreEqual(result.Length, response.Length);
            for (int i = 0; i < result.Length; i++)
            {
                Assert.IsTrue(response.Any(x => x == result[i]));
            }
        }

        [Test]
        public async Task TestForGet()
        {
            using var handle = server.StartServer();
            var destination = pathForData + "WorkWithVK2.dll";
            var pathForFile = pathForData + "WorkWithVK.dll";
            var result = File.ReadAllBytes(pathForFile);
            using (var fstream = new FileStream(destination, FileMode.OpenOrCreate))
            { 
                var response = await client.Get(pathForFile, fstream, tokenSource.Token);
                Assert.AreEqual(result.Length, response);
            }

            var result2 = File.ReadAllBytes(destination);
            Assert.AreEqual(result, result2);
            File.Delete(destination);
        } 
    }
}