using System;

namespace YuoTools
{
    public class PriorityQueue<T> where T : IComparable<T>
    {
        private T[] queue;
        private int count;
        private Func<T, T, bool> cmp;

        public PriorityQueue(int capacity = 100, Func<T, T, bool> cmp = null)
        {
            this.queue = new T[capacity];
            this.count = 0;
            this.cmp = cmp ?? ((x, y) => x.CompareTo(y) < 0);
        }

        public int Count => count;

        public T Peek()
        {
            if (count == 0)
            {
                throw new InvalidOperationException("队列为空");
            }

            return queue[1];
        }

        public void Enqueue(T val)
        {
            count++;
            if (count == queue.Length)
            {
                T[] tmp = new T[queue.Length * 2];
                for (int i = 0; i < count; i++)
                {
                    tmp[i] = queue[i];
                }

                queue = tmp;
            }

            queue[count] = val;
            Swim(count);
        }

        public T Dequeue()
        {
            if (count == 0)
            {
                throw new InvalidOperationException("队列为空");
            }

            T val = queue[1];
            queue[1] = queue[count];
            queue[count] = default(T);
            count--;
            Sinking(1);
            return val;
        }

        private void Swim(int index)
        {
            while (index > 1 && cmp(queue[index], queue[index / 2]))
            {
                Swap(index, index / 2);
                index /= 2;
            }
        }

        private void Sinking(int index)
        {
            while (index * 2 <= count)
            {
                int child = index * 2;
                if (child + 1 <= count && cmp(queue[child + 1], queue[child]))
                {
                    child++;
                }

                if (!cmp(queue[index], queue[child]))
                {
                    break;
                }

                Swap(index, child);
                index = child;
            }
        }

        private void Swap(int x, int y)
        {
            (queue[x], queue[y]) = (queue[y], queue[x]);
        }
    }
}