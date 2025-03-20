using UnityEngine;
namespace Rougelike2D
{
    [CreateAssetMenu(fileName = "Ground Moving Strategy", menuName = "Scriptable Objects/Strategy/LocomotionStrategy/GroundMoving Strategy")]
    public class GroundMovingStrategy : LocomotionStrategy
    {
        public GroundMovingStrategy(float moveSpeed) : base(moveSpeed)
        {
        }

        public override void HorizontalMovement()
        {
            
        }
    }
}
