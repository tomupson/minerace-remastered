using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace MineRace.Infrastructure
{
    public class MessageChannel<T> : IMessageChannel<T>
    {
        private readonly List<Action<T>> messageHandlers = new List<Action<T>>();

        // This dictionary of handlers to be either added or removed is used to prevent problems from immediate
        // modification of the list of subscribers. It could happen if one decides to unsubscribe in a message handler
        // etc.A true value means this handler should be added, and a false one means it should be removed
        private readonly Dictionary<Action<T>, bool> pendingHandlers = new Dictionary<Action<T>, bool>();

        public bool IsDisposed { get; private set; }

        public virtual void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                messageHandlers.Clear();
                pendingHandlers.Clear();
            }
        }

        public virtual void Publish(T message)
        {
            foreach ((Action<T> handler, bool add) in pendingHandlers)
            {
                if (add)
                {
                    messageHandlers.Add(handler);
                }
                else
                {
                    messageHandlers.Remove(handler);
                }
            }

            pendingHandlers.Clear();

            foreach (Action<T> messageHandler in messageHandlers)
            {
                messageHandler?.Invoke(message);
            }
        }

        public virtual IDisposable Subscribe(Action<T> handler)
        {
            Assert.IsTrue(!IsSubscribed(handler), "Attempting to subscribe with the same handler more than once");

            if (!pendingHandlers.TryGetValue(handler, out bool add))
            {
                pendingHandlers.Add(handler, true);
            }
            else if (!add)
            {
                pendingHandlers.Remove(handler);
            }

            return new DisposableSubscription<T>(this, handler);
        }

        public void Unsubscribe(Action<T> handler)
        {
            if (IsSubscribed(handler))
            {
                if (!pendingHandlers.TryGetValue(handler, out bool add))
                {
                    pendingHandlers.Add(handler, false);
                }
                else if (add)
                {
                    pendingHandlers.Remove(handler);
                }
            }
        }

        private bool IsSubscribed(Action<T> handler)
        {
            bool isPendingRemoval = pendingHandlers.ContainsKey(handler) && !pendingHandlers[handler];
            bool isPendingAdding = pendingHandlers.ContainsKey(handler) && pendingHandlers[handler];
            return (messageHandlers.Contains(handler) && !isPendingRemoval) || isPendingAdding;
        }
    }
}
