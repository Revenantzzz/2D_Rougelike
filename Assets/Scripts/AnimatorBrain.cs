using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Rougelike2D
{
    public class AnimatorBrain : MonoBehaviour
    {
        private Animator _animator;
        public string CurrentAnimation {get; private set;}
        public bool IsLocked {get; set;}
        private Action<int> DefaultAnimation;

        protected void Initialize( Animator animator, string startAnimation, Action<int> DefaultAnimation)
        {
            this._animator = animator;
            this.DefaultAnimation = DefaultAnimation;
            this.CurrentAnimation = startAnimation;
            this.IsLocked = false;
        }

        public void Play(string animation, bool isLocking, bool bypassLock, float crossfade = .05f)
        {
            if(animation == string.Empty)
        {
            DefaultAnimation(0);
            return;
        }

        if (IsLocked && !bypassLock) 
            return;

        IsLocked = isLocking;

        if(bypassLock)
        {
            foreach (var item in _animator.GetBehaviours<OnFinishAnimation>())
            {
                item.cancel = true;
            }
        }

        if (CurrentAnimation == animation) return;

        CurrentAnimation = animation;
        _animator.CrossFade(CurrentAnimation, crossfade);
        }

        public void SetFloatValue(string variableName, float value)
        {
            _animator.SetFloat(variableName, value);
        }
        public void SetBoolValue(string variableName, bool value)
        {
            _animator.SetBool(variableName, value);
        }
    }
}
