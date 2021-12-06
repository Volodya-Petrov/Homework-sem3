
using System;

namespace WorkWithFTP
{
    class Program
    {
        static void Main(string[] args)
        {
            string ip = args[0];
            var port = int.Parse(args[1]);
            try
            {
                var server = new Server(ip, port);
                using var handler = server.StartServer();
                Console.WriteLine("Введите stop, чтобы остановить сервер");
                var command = "";
                while (command != "stop")
                {
                    command = Console.ReadLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Упс... что-то пошло не так");
            }
        }
    }
}