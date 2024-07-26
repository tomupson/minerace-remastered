using System;

namespace MineRace.Infrastructure
{
    public sealed class BufferedMessageChannel<T> : MessageChannel<T>
    {
        private static BufferedMessageChannel<T> instance;

        public static new BufferedMessageChannel<T> Instance => instance ??= new BufferedMessageChannel<T>();

        public bool HasBufferedMessage { get; private set; }

        public T BufferedMessage { get; private set; }

        public override void Publish(T message)
        {
            HasBufferedMessage = true;
            BufferedMessage = message;
            base.Publish(message);
        }

        public override IDisposable Subscribe(Action<T> handler)
        {
            IDisposable subscription = base.Subscribe(handler);

            if (HasBufferedMessage)
            {
                handler?.Invoke(BufferedMessage);
            }

            return subscription;
        }
    }
}
