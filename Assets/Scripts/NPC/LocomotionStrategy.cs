using UnityEngine;

namespace Rougelike2D
{
    public abstract class LocomotionStrategy : ScriptableObject
    {
        protected float moveSpeed;

        public LocomotionStrategy(float moveSpeed)
        {
            this.moveSpeed = moveSpeed;
        }
        public abstract void HorizontalMovement();
    }
}
