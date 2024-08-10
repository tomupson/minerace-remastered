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
        private ISubscriber<T> subscriber;

        public DisposableSubscription(ISubscriber<T> subscriber, Action<T> handler)
        {
            this.subscriber = subscriber;
            this.handler = handler;
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;

                subscriber.Unsubscribe(handler);

                handler = null;
                subscriber = null;
            }
        }
    }
}
