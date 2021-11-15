
using System;

namespace WorkWithFTP
{
    class Program
    {
        static void Main(string[] args)
        {   
            Console.WriteLine("Введите ip, на котором будет запущен сервер:");
            var ip = Console.ReadLine();
            Console.WriteLine("Введите порт сервера:");
            var port = int.Parse(Console.ReadLine());
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