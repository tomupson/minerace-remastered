using System;

namespace MineRace.Infrastructure
{
    /// <summary>
    /// This class is a handle to an active Message Channel subscription and when disposed it unsubscribes from said channel.
    /// </summary>
    public sealed class DisposableSubscription<T> : IDisposable
    {
        private Action<T> handler;
        private bool isDisposed;
        private IMessageChannel<T> messageChannel;

        public DisposableSubscription(IMessageChannel<T> messageChannel, Action<T> handler)
        {
            this.messageChannel = messageChannel;
            this.handler = handler;
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;

                if (!messageChannel.IsDisposed)
                {
                    messageChannel.Unsubscribe(handler);
                }

                handler = null;
                messageChannel = null;
            }
        }
    }
}
