using System;
using System.Text;

namespace CheckSum
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = Console.ReadLine();
            var checkSum = new CheckSum();
            var result = checkSum.Calculate(path);
            Console.WriteLine(BitConverter.ToString(result));
        }
    }
}