using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace WorkWithFTP
{   
    /// <summary>
    /// Класс сервера, реализующего протокол FTP
    /// </summary>
    public class Server
    {
        private readonly int port;
        private readonly string IPAdress;
        private CancellationTokenSource tokenSource;

        public Server(string ipAdress, int port)
        {
            IPAdress = ipAdress;
            this.port = port;
            
        }

        enum Request
        {
            List = 1,
            Get,
            Nonsense
        }
        
        /// <summary>
        /// Запускает сервер
        /// </summary>
        public async Task StartServer()
        {
            var listener = new TcpListener(IPAddress.Parse(IPAdress), port);
            listener.Start();
            tokenSource = new CancellationTokenSource();
            while (!tokenSource.Token.IsCancellationRequested)
            {
                var client = await listener.AcceptTcpClientAsync();
                Task.Run(() => WorkWithClient(client));
            }
            listener.Stop();
        }

        /// <summary>
        /// Завершает работу сервера
        /// </summary>
        public void StopServer() => tokenSource.Cancel();
        
        private async void WorkWithClient(TcpClient client)
        {
            using var stream = client.GetStream();
            var reader = new StreamReader(stream);
            var writer = new StreamWriter(stream);
            var data = await reader.ReadLineAsync();
            (var request, var path) = ParseData(data);
            switch (request)
            {
                case Request.Nonsense:
                    await writer.WriteAsync("Bro you broke protocol, don't do that anymore, please");
                    await writer.FlushAsync();
                    break;
                case Request.List:
                    await ResponseForList(path, writer);
                    break;
                default:
                    await ResponseForGet(path, writer, stream);
                    break;
            }
        }

        private (Request, string) ParseData(string data)
        {
            if (data[0] != '1' && data[0] != '2')
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

        private async Task ResponseForList(string path, StreamWriter writer)
        {
            if (!Directory.Exists(path))
            {
                await writer.WriteLineAsync("-1 Directory doesn't exist");
                await writer.FlushAsync();
                return;
            }
            var files = Directory.GetFiles(path);
            var directories = Directory.GetDirectories(path);
            var result = (files.Length + directories.Length).ToString();
            files.ToList().ForEach(x => { result += $" {x} False";});
            directories.ToList().ForEach(x => { result += $" {x} True";});
            await writer.WriteLineAsync(result);
            await writer.FlushAsync();
        }

        private async Task ResponseForGet(string path, StreamWriter writer, NetworkStream stream)
        {
            if (!File.Exists(path))
            {
                await writer.WriteLineAsync("-1 File doesn't exist");
                await writer.FlushAsync();
            }

            var fileStream = new FileStream(path, FileMode.Open);
            await writer.WriteAsync(fileStream.Length.ToString() + " ");
            await writer.FlushAsync();
            await fileStream.CopyToAsync(stream);
            await stream.FlushAsync();
        }
    }
}