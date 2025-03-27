using UnityEngine;

namespace Rougelike2D
{
    public class PlayerMoveState : BaseState
    {
        public PlayerMoveState(PlayerController controller, AnimatorBrain animatorBrain) : base(controller, animatorBrain)
        {
        }

        public override void EnterState()
        {
            base.EnterState();
            animatorBrain.Play(AnimationString.PlayerRunning);
        }
        public override void StateUpdate()
        {
            base.StateUpdate();  
        }
        public override void StateFixedUpdate()
        {
            base.StateFixedUpdate();
            playerController.HandleMovement();
            if(playerController.IsMovingInput)
            {
                animatorBrain.Play(AnimationString.PlayerRunning);
            }  
            else
            {
                animatorBrain.Play(AnimationString.PlayerIdle);
            }
        }
    }
}
