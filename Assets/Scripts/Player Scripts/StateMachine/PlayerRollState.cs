using UnityEngine;

namespace Rougelike2D
{
    public class PlayerRollState : BaseState
    {
        public PlayerRollState(PlayerController controller, AnimatorBrain animatorBrain) : base(controller, animatorBrain)
        {
        }

        public override void EnterState()
        {
            playerController.StopMovement();
            base.EnterState();
            playerController.HandleRoll();
            animatorBrain.Play("Player_Roll");
        }
        public override void StateFixedUpdate()
        {
            base.StateFixedUpdate(); 
        }
        public override void ExitState()
        {
            base.ExitState();
            playerController.StopMovement();
        }
    }
}
