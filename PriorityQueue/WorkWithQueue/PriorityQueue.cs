using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace WorkWithQueue
{
    /// <summary>
    /// Потокобезопасная очередь с приоритетами
    /// </summary>
    public class PriorityQueue
    {
        private List<QueueElement> queue;
        
        private class QueueElement
        {
            public int Priority { get; set; }
            public int Value { get; set; }
        }

        /// <summary>
        /// Возвращает размер очереди в какой-то момент времени
        /// </summary>
        public int Size => queue.Count;

        /// <summary>
        /// Добавляет значение в очередь
        /// </summary>
        public void Enqueue(int value, int priority)
        {
            var element = new QueueElement() {Priority = priority, Value = value};
            lock (queue)
            {
                for (int i = 0; i < queue.Count; i++)
                {
                    if (queue[i].Priority < priority)
                    {
                        queue.Insert(i, element);
                        Monitor.PulseAll(queue);
                        return;
                    }
                }

                queue.Add(element);
                Monitor.PulseAll(queue);
            }
        }
        
        /// <summary>
        /// Возвращает элемент с наибольшим приоритетом в очереди
        /// </summary>
        public int Dequeue()
        {
            lock (queue)
            {
                while (queue.Count == 0)
                {
                    Monitor.Wait(queue);
                }

                var value = queue[0].Value;
                queue.RemoveAt(0);
                return value;
            }
        }
    }
}