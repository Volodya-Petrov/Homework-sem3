using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Test1._1
{   
    /// <summary>
    /// Класс клиента сетевого чата
    /// </summary>
    public class Client
    {
        private string ip;
        private int port;
        private StreamReader readStream;
        private StreamWriter writeStream;
        
        public Client(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
        }
        
        /// <summary>
        /// Подключение к серверу
        /// </summary>
        public async Task Connect()
        {
            var tcpClient = new TcpClient(ip, port);
            var stream = tcpClient.GetStream();
            readStream = new StreamReader(stream);
            writeStream = new StreamWriter(stream);
        }
        
        /// <summary>
        /// Отправить сообщение собеседнику
        /// </summary>
        public async Task Send(string message) =>
            await writeStream.WriteLineAsync(message);
        
        /// <summary>
        /// Читает сообщение собеседника
        /// </summary>
        public async Task<string> Read() =>
            await readStream.ReadLineAsync();
    }
}