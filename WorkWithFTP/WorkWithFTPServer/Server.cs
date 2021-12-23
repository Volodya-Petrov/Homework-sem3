using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
        private List<Task> tasks;
        private TcpListener listener;

        private class ServerHandler : IDisposable
        {
            private Action stopServer;
            
            public ServerHandler(Action stopServer)
            {
                this.stopServer = stopServer;
            }
            
            public void Dispose()
            {
                stopServer();
            }
        }
        
        public Server(string ipAdress, int port)
        {
            IPAdress = ipAdress;
            this.port = port;
            tasks = new();
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
        public IDisposable StartServer()
        {
            listener = new TcpListener(IPAddress.Parse(IPAdress), port);
            listener.Start();
            tokenSource = new CancellationTokenSource();
            var mainTask = Task.Run(async () =>
            {
                while (!tokenSource.Token.IsCancellationRequested)
                {
                    var client = await listener.AcceptTcpClientAsync();
                    var task = Task.Run(() => WorkWithClient(client));
                    tasks.Add(task);
                }
            });
            tasks.Add(mainTask);
            return new ServerHandler(StopServer);
        }

        /// <summary>
        /// Завершает работу сервера
        /// </summary>
        public void StopServer()
        {
            tokenSource.Cancel();
            Task.WhenAll(tasks);
            listener.Stop();
        }
        
        private async void WorkWithClient(TcpClient client)
        {
            using (client)
            {
                using var stream = client.GetStream();
                using var reader = new StreamReader(stream);
                using var writer = new StreamWriter(stream) {AutoFlush = true};
                var data = await reader.ReadLineAsync(); 
                (var request, var path) = ParseData(data);
                switch (request)
                {
                    case Request.Nonsense:
                        await writer.WriteAsync("Bro you broke protocol, don't do that anymore, please");
                        break;
                    case Request.List:
                        await ResponseForList(path, writer);
                        break;
                    default:
                        await ResponseForGet(path, writer, stream);
                        break;
                }   
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
                return;
            }
            var files = Directory.GetFiles(path);
            var directories = Directory.GetDirectories(path);
            var response = new StringBuilder();
            response.Append((files.Length + directories.Length).ToString());
            files.ToList().ForEach(x => response.Append($" {x} False"));
            directories.ToList().ForEach(x => response.Append($" {x} True"));
            await writer.WriteLineAsync(response.ToString());
        }

        private async Task ResponseForGet(string path, StreamWriter writer, NetworkStream stream)
        {
            if (!File.Exists(path))
            {
                await writer.WriteLineAsync("-1 File doesn't exist");
                return;
            }

            using var fileStream = new FileStream(path, FileMode.Open);
            await writer.WriteAsync(fileStream.Length.ToString() + " ");
            await fileStream.CopyToAsync(stream);
            await stream.FlushAsync();
        }
    }
}