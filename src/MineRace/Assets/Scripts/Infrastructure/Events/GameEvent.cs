using System;
using System.Collections.Generic;
using UnityEngine;

namespace MineRace.Infrastructure
{
    public abstract class GameEvent<T> : ScriptableObject
    {
        private readonly List<Action<T>> actions = new();

        public void Raise(T data)
        {
            for (int actionIdx = actions.Count - 1; actionIdx >= 0; actionIdx--)
            {
                if (actions[actionIdx] == null)
                {
                    actions.RemoveAt(actionIdx);
                    continue;
                }

                actions[actionIdx].Invoke(data);
            }
        }

        public void RegisterListener(Action<T> action)
        {
            if (actions.Contains(action))
            {
                return;
            }

            actions.Add(action);
        }

        public void DeregisterListener(Action<T> action)
        {
            actions.Remove(action);
        }
    }
}
