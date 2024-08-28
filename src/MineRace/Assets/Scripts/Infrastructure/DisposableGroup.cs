using System;
using System.Collections.Generic;

namespace MineRace.Infrastructure
{
    public sealed class DisposableGroup : IDisposable
    {
        private readonly List<IDisposable> disposables = new();

        public void Dispose()
        {
            foreach (IDisposable disposable in disposables)
            {
                disposable.Dispose();
            }

            disposables.Clear();
        }

        public void Add(IDisposable disposable)
        {
            disposables.Add(disposable);
        }
    }
}
