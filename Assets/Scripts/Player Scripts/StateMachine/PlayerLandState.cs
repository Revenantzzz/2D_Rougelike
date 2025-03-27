using UnityEngine;

namespace Rougelike2D
{
    public class PlayerLandState : BaseState
    {
        public PlayerLandState(PlayerController controller, AnimatorBrain animatorBrain) : base(controller, animatorBrain)
        {
        }
        public override void EnterState()
        {
            base.EnterState();
            animatorBrain.Play(AnimationString.PlayerLand);
            playerController.StopMovement();
        }
        public override void ExitState()
        {
            base.ExitState();
            playerController.StopMovement();
        }
    }
}
