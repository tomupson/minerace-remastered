using UnityEngine;

namespace MineRace.Utils.Animation
{
    public class AnimationStateDispatchBehaviour : StateMachineBehaviour
    {
        private IAnimationStateHandler animationStateHandler;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animationStateHandler ??= animator.GetComponentInParent<IAnimationStateHandler>();
            animationStateHandler?.OnStateEnter(animator, stateInfo, layerIndex);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animationStateHandler ??= animator.GetComponentInParent<IAnimationStateHandler>();
            animationStateHandler?.OnStateUpdate(animator, stateInfo, layerIndex);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animationStateHandler ??= animator.GetComponentInParent<IAnimationStateHandler>();
            animationStateHandler?.OnStateExit(animator, stateInfo, layerIndex);
        }
    }
}
