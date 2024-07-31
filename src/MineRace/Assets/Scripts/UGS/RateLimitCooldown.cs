using UnityEngine;

namespace MineRace.UGS
{
    internal sealed class RateLimitCooldown
    {
        private readonly float cooldownTimeLength;
        private float cooldownFinishTime;

        public RateLimitCooldown(float cooldownTimeLength)
        {
            this.cooldownTimeLength = cooldownTimeLength;
            cooldownFinishTime = -1f;
        }

        public bool CanCall => Time.unscaledTime > cooldownFinishTime;

        public void PutOnCooldown()
        {
            cooldownFinishTime = Time.unscaledTime + cooldownTimeLength;
        }
    }
}