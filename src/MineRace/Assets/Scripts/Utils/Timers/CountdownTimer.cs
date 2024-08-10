using System;

namespace MineRace.Utils.Timers
{
    public sealed class CountdownTimer
    {
        private readonly float initialTime;

        public float Time { get; private set; }

        public bool IsRunning { get; private set; }

        public Action OnTimerStart;
        public Action OnTimerStop;

        public CountdownTimer(float initialTime)
        {
            this.initialTime = initialTime;
        }

        public void Start()
        {
            Time = initialTime;
            if (!IsRunning)
            {
                IsRunning = true;
                OnTimerStart?.Invoke();
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                OnTimerStop?.Invoke();
            }
        }

        public void Resume() => IsRunning = true;

        public void Pause() => IsRunning = false;

        public void Tick(float deltaTime)
        {
            if (IsRunning && Time > 0)
            {
                Time -= deltaTime;
            }

            if (IsRunning && Time <= 0)
            {
                Stop();
            }
        }

        public void Reset() => Time = initialTime;
    }
}
