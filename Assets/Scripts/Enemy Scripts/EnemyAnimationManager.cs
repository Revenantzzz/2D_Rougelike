using UnityEngine;

namespace Rougelike2D
{
    public class EnemyAnimationManager : AnimatorBrain
    {
        void Start()
        {
            Initialize(GetComponent<Animator>(), AnimationString.SkeletonIdle, DefaultAnimation);
        }
        private void DefaultAnimation(int num)
        {
            Play(AnimationString.SkeletonIdle);
        }
    }
}
