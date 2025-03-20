using UnityEngine;
namespace Rougelike2D
{
    [CreateAssetMenu(fileName = "StaticStrategy", menuName = "Scriptable Objects/Strategy/LocomotionStrategy/Static Strategy")]
    public class StaticStrategy : LocomotionStrategy
    {
        public StaticStrategy(float moveSpeed) : base(moveSpeed)
        {
            this.moveSpeed = 0f;
        }

        public override void HorizontalMovement()
        {
            //Do nothing, this NPC cant move
        }
    }
}
