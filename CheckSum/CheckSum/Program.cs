using System;
using System.Text;

namespace CheckSum
{
    class Program
    {
        static void Main(string[] args)
        {
            var checkSum = new CheckSum();
            var result = checkSum.Calculate(args[0]);
            Console.WriteLine(BitConverter.ToString(result));
        }
    }
}