namespace MineRace.Utils.Netcode
{
    public sealed class NetworkTimer
    {
        private float timer;

        public float MinTimeBetweenTicks { get; }

        public int CurrentTick { get; private set; }

        public NetworkTimer(float serverTickRate)
        {
            MinTimeBetweenTicks = 1f / serverTickRate;
        }

        public void Tick(float deltaTime)
        {
            timer += deltaTime;
        }

        public bool ShouldTick()
        {
            if (timer >= MinTimeBetweenTicks)
            {
                timer -= MinTimeBetweenTicks;
                CurrentTick++;
                return true;
            }

            return false;
        }
    }
}