using System.Collections.Generic;

namespace CodeChallenge.Extensions
{
    public static class QueueExtensions
    {
        public static void EnqueueAll<T>(this Queue<T> queue, IEnumerable<T> items)
        {
            if (items != null)
            {
                foreach (T item in items)
                {
                    queue.Enqueue(item);
                }
            }
        }
    }
}
