using System;
using System.Dynamic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WorkWithFTPClient
{   
    /// <summary>
    /// Клиент сервера
    /// </summary>
    public class Client
    {
        private readonly int port;
        private readonly string hostName;
        
        public Client(string hostName, int port)
        {
            this.hostName = hostName;
            this.port = port;
        }
        
        /// <summary>
        /// Скачивает файл с сервера
        /// </summary>
        public async Task<long> Get(string path, FileStream destination, CancellationToken token)
        { 
            using var client = new TcpClient();
            await client.ConnectAsync(hostName, port, token);
            using var stream = client.GetStream();
            using var writer = new StreamWriter(stream){AutoFlush = true};
            using var reader = new StreamReader(stream);
            await writer.WriteLineAsync($"2 {path}");
            var size = new char[long.MaxValue.ToString().Length + 1];
            var index = 0;
            await reader.ReadAsync(size, index, 1);
            if (size[0] == '-')
            {
                throw new ArgumentException();
            }
            while (size[index] != ' ')
            {
                index++;
                await reader.ReadAsync(size, index, 1);
            }

            var countInString = "";
            for (int i = 0; i < index; i++)
            {
                countInString += size[i].ToString();
            }
            var count = long.Parse(countInString);
            await stream.CopyToAsync(destination, token);
            return count;
        }
        
        /// <summary>
        /// Показывает все папки и файлы, которые лежат на сервере по заданному пути
        /// </summary>
        public async Task<(string, bool)[]> List(string path, CancellationToken token)
        {
            using var client = new TcpClient();
            await client.ConnectAsync(hostName, port, token);
            using var stream = client.GetStream();
            using var writer = new StreamWriter(stream){AutoFlush = true};
            using var reader = new StreamReader(stream);
            await writer.WriteLineAsync($"1 {path}");
            var response = await reader.ReadLineAsync();
            var splittedResponse = response.Split(' ');
            if (splittedResponse[0] == "-1")
            {
                throw new ArgumentException();
            }
            var countOfFiles = int.Parse(splittedResponse[0]);
            var files = new (string, bool)[countOfFiles];
            for (int i = 1; i < splittedResponse.Length; i += 2)
            {
                var isDir = splittedResponse[i + 1] == "True";
                files[(i - 1) / 2] = (splittedResponse[i], isDir);
            }

            return files;
        }
    }
}