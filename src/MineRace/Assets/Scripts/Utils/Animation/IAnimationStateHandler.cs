using UnityEngine;

namespace MineRace.Utils.Animation
{
    public interface IAnimationStateHandler
    {
        void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex);
        void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex);
        void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex);
    }
}
