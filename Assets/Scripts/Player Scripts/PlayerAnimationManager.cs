using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Rougelike2D
{
    public class PlayerAnimationManager : AnimatorBrain
    {

        void Awake()
        {
            Initialize(GetComponent<Animator>(), AnimationString.PlayerIdle, DefaultAnimation);
        }
        private void DefaultAnimation(int num)
        {
            Play(AnimationString.PlayerIdle);
        }

    }   
}
