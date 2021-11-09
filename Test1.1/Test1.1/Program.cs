using System;
using System.Threading;
using System.Threading.Tasks;

namespace Test1._1
{   
    class Program
    {
        private static ManualResetEvent eventProgram;
        static async Task Main(string[] args)
        {
            eventProgram = new ManualResetEvent(false);
            Console.WriteLine("Если вы хотитет зайти за\n" +
                              "1) Клиент: пропишите IP адресс и порт сервера" +
                              "2) Сервер: пропишите порт");
            var connectionString = Console.ReadLine().Split(' ');
            int.TryParse(connectionString[0], out int port);
            var threads = new Thread[2];
            if (connectionString.Length == 1)
            {
                var server = new Server(port);
                await server.Start();
                Console.WriteLine("Пользователь подключился к серверу");
                threads[0] = new Thread(async () =>
                {
                    while (true)
                    {
                        var text = Console.ReadLine();
                        await server.SendMessage(text);
                        if (text == "exit")
                        {
                            server.Stop();
                            eventProgram.Set();
                        }
                    }
                });
                threads[1] = new Thread(async () =>
                {
                    while (true)
                    {
                        var text = await server.ReadMessage();
                        Console.WriteLine(text);
                        if (text == "exit")
                        {
                            server.Stop();
                            eventProgram.Set();
                        }
                    }
                });
            }
            else
            {
                var client = new Client(connectionString[1], port);
                threads[0] = new Thread(async () =>
                {
                    while (true)
                    {
                        var text = Console.ReadLine();
                        await client.Send(text);
                        if (text == "exit")
                        {
                            eventProgram.Set();
                        }
                    }
                });
                threads[1] = new Thread(async () =>
                {
                    while (true)
                    {
                        var text = await client.Read();
                        Console.WriteLine(text);
                        if (text == "exit")
                        {
                            eventProgram.Set();
                        }
                    }
                }); 
            }

            threads[0].Start();
            threads[1].Start();
            eventProgram.WaitOne();
        }
    }
}