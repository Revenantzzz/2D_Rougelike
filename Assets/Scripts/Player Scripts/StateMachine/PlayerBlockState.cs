using UnityEngine;

namespace Rougelike2D
{
    public class PlayerBlockState : BaseState
    {
        public PlayerBlockState(PlayerController controller, AnimatorBrain animatorBrain) : base(controller, animatorBrain)
        {
        }

        public override void EnterState()
        {
            base.EnterState();
            playerController.HorizontalMovement.StopMovement();
            animatorBrain.Play("Player_ToBlock");
        }
        public override void StateFixedUpdate()
        {
            base.StateFixedUpdate();
            animatorBrain.Play("Player_Blocking");
        }
        public override void ExitState()
        {
            base.ExitState();
            animatorBrain.Play("Player_OutBlock");
        }
    }
}
