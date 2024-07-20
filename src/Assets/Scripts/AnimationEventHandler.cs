using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationEventHandler : MonoBehaviour
{
    public event Action OnAnimationComplete;

    private void NotifyAnimationComplete()
    {
        OnAnimationComplete?.Invoke();
    }
}
