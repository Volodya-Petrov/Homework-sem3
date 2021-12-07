using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CheckSum
{   
    /// <summary>
    /// Класс для подсчета хэша директории
    /// </summary>
    public class CheckSum
    {   
        /// <summary>
        /// Считает хэш директории
        /// </summary>
        public byte[] Calculate(string path)
        {
            var files = Directory.GetFiles(path);
            var directories = Directory.GetDirectories(path);
            var tasks = new Task<byte[]>[directories.Length + files.Length];
            for (int i = 0; i < directories.Length; i++)
            {
                var index = i;
                tasks[i] = Task.Run(() => Calculate(directories[index]));
            }

            for (int i = 0; i < files.Length; i++)
            {
                var index = i;
                tasks[directories.Length + index] = Task.Run(() => GetHashFromFile(files[index]));
            }

            long sum = 0;
            for (int i = 0; i < tasks.Length; i++)
            {
                sum += BitConverter.ToInt64(tasks[i].Result);
            }

            sum += Path.GetDirectoryName(path).Length;
            return BitConverter.GetBytes(sum);
        }

        private byte[] GetHashFromFile(string path)
        {
            var hashCalculator = MD5.Create(); 
            using Stream stream = new FileStream(path, FileMode.Open);
            return hashCalculator.ComputeHash(stream);
        }
    }   
}