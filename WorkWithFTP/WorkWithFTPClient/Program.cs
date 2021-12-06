using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WorkWithFTPClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new Client(args[0], int.Parse(args[1]));
            Console.WriteLine("List:\nФормат запроса:1 <path: String>\npath - путь к директории относительно того места, где запущен сервер");
            Console.WriteLine("Get:\nФормат запроса:2 <path1: String> <path2: String>\npath1 - путь к файлу относительно того места, где запущен сервер\n" +
                              "npath2 - путь к файлу на локальной машине, где запущен клиент");
            Console.WriteLine("Введите exit, если хотите закрыть соединение с сервером");
            var request = Console.ReadLine().Split(' ');
            while (request[0] != "exit") 
            {
                if (request[0] == "1")
                {
                    try
                    {
                        var sourceToken = new CancellationTokenSource();
                        var response = await client.List(request[1], sourceToken.Token);
                        foreach (var file in response)
                        {
                            Console.WriteLine($"{file.Item1} {file.Item2}");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Упс... Что-то пошло не так");
                    }
                }

                if (request[0] == "2")
                {
                    using (var fstream = new FileStream(request[2], FileMode.OpenOrCreate))
                    {
                        try
                        {
                            var sourceToken = new CancellationTokenSource();
                            var response = client.Get(request[1], fstream, sourceToken.Token);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Упс... Что-то пошло не так");
                        }
                    }
                }
                request = Console.ReadLine().Split(' ');
            }
        }
    }
}