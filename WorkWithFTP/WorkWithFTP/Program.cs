using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace WorkWithFTP
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new MyFTP(8888);
            server.Run();
            for (int i = 0; i < 3; i++)
            {
                var index = i;
                Task.Run(async () =>
                {
                    var client = new TcpClient("localhost", 8888);
                    var stream = client.GetStream();
                    var streamWriter = new StreamWriter(stream);
                    var streamReader = new StreamReader(stream);
                    await streamWriter.WriteAsync("1 .");
                    var text = await streamReader.ReadLineAsync();
                    Console.WriteLine($"это {index} крутой чувак\n{text}");
                });
            }
            Thread.Sleep(100000);
        }
    }
}