using System;
using System.Collections.Generic;
using UnityEngine;

namespace MineRace.Infrastructure
{
    [CreateAssetMenu(fileName = "Event", menuName = "Events/Void")]
    public class VoidGameEvent : ScriptableObject
    {
        private readonly List<Action> actions = new();

        public void Raise()
        {
            for (int actionIdx = actions.Count - 1; actionIdx >= 0; actionIdx--)
            {
                if (actions[actionIdx] == null)
                {
                    actions.RemoveAt(actionIdx);
                    continue;
                }

                actions[actionIdx].Invoke();
            }
        }

        public void RegisterListener(Action action)
        {
            if (actions.Contains(action))
            {
                return;
            }

            actions.Add(action);
        }

        public void DeregisterListener(Action action)
        {
            actions.Remove(action);
        }
    }
}
