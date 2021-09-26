using System;
using System.Threading;

namespace DeadlockCathcer
{
    class Program
    {
        private static int countOfPhilosophers = 2;

        static void Main(string[] args)
        {
            var threads = new Thread[countOfPhilosophers];
            var forks = new Fork[countOfPhilosophers];
            for (int i = 0; i < forks.Length; i++)
            {
                forks[i] = new Fork();
                forks[i].Id = i;
            }
            
            for (int i = 0; i < threads.Length; i++)
            {
                var index = i;
                threads[i] = new Thread(() =>
                {
                    var random = new Random();
                    var philosopher = new Philosopher(index);
                    philosopher.Live(random, forks[index], forks[(index + 1) % countOfPhilosophers]);
                });
                threads[i].Start();
            }
        }
    }
}