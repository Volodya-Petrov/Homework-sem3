using System.Dynamic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WorkWithFTPClient
{
    public class Client
    {
        private readonly int port;
        private readonly string hostName;
        
        public Client(string hostName, int port)
        {
            this.hostName = hostName;
            this.port = port;
        }

        public async Task<(long, byte[])> Get(string path)
        {
            var client = new TcpClient(hostName, port);
            using var stream = client.GetStream();
            var writer = new StreamWriter(stream);
            var reader = new StreamReader(stream);
            await writer.WriteLineAsync($"2 {path}");
            await writer.FlushAsync();
            var size = new char[long.MaxValue.ToString().Length + 1];
            var index = 0;
            await reader.ReadAsync(size, index, 1);
            while (size[index] != ' ')
            {
                index++;
                await reader.ReadAsync(size, index, 1);
            }

            var countInString = size.ToString().Substring(0, size.Length - 1);
            var count= long.Parse(countInString);
            var bytes = new byte[count];
            for (int i = 0; i < count; i++)
            {
                await stream.ReadAsync(bytes, i, 1);
            }

            return (count, bytes);
        }

        public async Task<(string, bool)[]> List(string path)
        {
            var client = new TcpClient(hostName, port);
            using var stream = client.GetStream();
            var writer = new StreamWriter(stream);
            var reader = new StreamReader(stream);
            await writer.WriteLineAsync($"1 {path}");
            await writer.FlushAsync();
            var response = await reader.ReadLineAsync();
            var splittedResponse = response.Split(' ');
            var countOfFiles = int.Parse(splittedResponse[0]);
            var files = new (string, bool)[countOfFiles];
            for (int i = 1; i < countOfFiles; i += 2)
            {
                var isDir = splittedResponse[i + 1] == "True";
                files[(i - 1) / 2] = (splittedResponse[i], isDir);
            }

            return files;
        }
    }
}