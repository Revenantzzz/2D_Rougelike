using UnityEngine;

namespace Rougelike2D
{
    public class PlayerClimbLedgeState : BaseState
    {
        public PlayerClimbLedgeState(PlayerController controller, AnimatorBrain animatorBrain) : base(controller, animatorBrain)
        {
        }
        public override void EnterState()
        {
            playerController.HorizontalMovement.StopMovement();
            base.EnterState();
            animatorBrain.Play("Player_LedgeGrab");
        }
        public override void StateUpdate()
        {
            playerController.LedgeGrabbing();
            base.StateUpdate();
        }
    }
}
