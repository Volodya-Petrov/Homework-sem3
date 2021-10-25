using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace WorkWithFTP
{
    public class MyFTP
    {
        private readonly int port;

        public MyFTP(int port)
        {
            this.port = port;
        }

        enum Request
        {
            List = 1,
            Get,
            Nonsense
        }

        public void Run()
        {
            Task.Run(async () => StartServer());
        }
        
        private async void StartServer()
        {
            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            while (true)
            {
                var socket = await listener.AcceptSocketAsync();
                Task.Run(async () => WorkWithClient(socket));
            }
        }

        private async void WorkWithClient(Socket socket)
        {
            var stream = new NetworkStream(socket);
            var reader = new StreamReader(stream);
            var writer = new StreamWriter(stream);
            while (!reader.EndOfStream)
            {
                var data = await reader.ReadLineAsync();
                (var request, var path) = ParseData(data);
                switch (request)
                {
                    case Request.Nonsense:
                        await writer.WriteAsync("Bro you broke protocol, don't do that anymore, please");
                        await writer.FlushAsync();
                        break;
                    case Request.List:
                        await writer.WriteAsync(ResponseForList(path));
                        await writer.FlushAsync();
                        break;
                    default:
                        var bytes = ResponseForGet(path);
                        if (bytes == null)
                        {
                            await writer.WriteAsync("-1 Файл не существует");
                            await writer.FlushAsync();
                            break;
                        }

                        await writer.WriteAsync($"{bytes.Length}");
                        await writer.FlushAsync();
                        stream.Write(bytes);
                        await stream.FlushAsync();
                        break;
                }
            }
        }

        private (Request, string) ParseData(string data)
        {
            if (data[0] != '1' || data[0] != '2')
            {
                return (Request.Nonsense, "");
            }

            var request = data[0] == '1' ? Request.List : Request.Get;
            if (data[1] != ' ')
            {
                return (Request.Nonsense, "");
            }

            var path = data.Substring(2);
            return (request, path);
        }

        private string ResponseForList(string path)
        {
            if (!Directory.Exists(path))
            {
                return "-1 Директории не существует"; 
            }
            var files = Directory.GetFiles(path);
            var directories = Directory.GetDirectories(path);
            var result = (files.Length + directories.Length).ToString();
            files.ToList().ForEach(x => { result += $" {x} False";});
            directories.ToList().ForEach(x => { result += $" {x} True";});
            return result;
        }

        private byte[] ResponseForGet(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            return File.ReadAllBytes(path);
        }
}
}