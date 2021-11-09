using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Test1._1
{   
    /// <summary>
    /// класс сервера сетевого чата
    /// </summary>
    public class Server
    {
        private int port;
        private TcpListener listener;
        private StreamReader readStream;
        private StreamWriter writeStream;

        public Server(int port)
        {
            this.port = port;
        }
        
        /// <summary>
        /// Запускает серве
        /// </summary>
        public async Task Start()
        {
            var listener = new TcpListener(IPAddress.Any, port);
            var socket = await listener.AcceptSocketAsync();
            var stream = new NetworkStream(socket); readStream = new StreamReader(stream);
            writeStream = new StreamWriter(stream) {AutoFlush = true};
        }
        
        /// <summary>
        /// Отправляет сообщение собеседнику
        /// </summary>
        public async Task SendMessage(string message) =>
            await writeStream.WriteLineAsync(message);
        
        /// <summary>
        /// Читает сообщение собеседника
        /// </summary>
        public async Task<string> ReadMessage() =>
            await readStream.ReadLineAsync();
        
        /// <summary>
        /// Остановить сервер
        /// </summary>
        public async Task Stop()
        {
            listener.Stop();
        }
    }
}