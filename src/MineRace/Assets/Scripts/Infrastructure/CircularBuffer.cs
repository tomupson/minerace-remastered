namespace MineRace.Infrastructure
{
    public class CircularBuffer<T>
    {
        private readonly int bufferSize;
        private T[] buffer;

        public CircularBuffer(int bufferSize)
        {
            this.bufferSize = bufferSize;
            buffer = new T[bufferSize];
        }

        public void Add(T item, int index) => buffer[index % bufferSize] = item;

        public T Get(int index) => buffer[index % bufferSize];

        public void Clear() => buffer = new T[bufferSize];
    }
}