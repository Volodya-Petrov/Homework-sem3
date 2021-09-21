using System;
using System.Threading;

namespace DeadlockCathcer
{
    public class Philosopher
    {
        public Philosopher(int numberOfPhilosopher)
        {
            NumberOfPhilosopher = numberOfPhilosopher;
        }

        private static int TimeForThinking = 100;
        
        public int NumberOfPhilosopher { get; }

        public void GetForksAndEat(Fork fork1, Fork fork2, Random random)
        {   
            Console.WriteLine($"Философ {this.NumberOfPhilosopher} хочет взять вилку 1");
            lock (fork1)
            {
                Console.WriteLine($"Философ {this.NumberOfPhilosopher} хочет взять вилку 2");
                lock (fork2)
                {
                    Console.WriteLine($"Философ {this.NumberOfPhilosopher} ест");
                    Thread.Sleep(random.Next(0, TimeForThinking));
                }
            }
        }

        public void Live(Random random, Fork fork1, Fork fork2)
        {
            if (fork1.Id > fork2.Id)
            {
                var helper = fork1;
                fork1 = fork2;
                fork2 = helper;
            }
            
            while (true)
            {
                Thread.Sleep(random.Next(0, TimeForThinking));
                GetForksAndEat(fork1, fork2, random);
            }
        }
    }
}