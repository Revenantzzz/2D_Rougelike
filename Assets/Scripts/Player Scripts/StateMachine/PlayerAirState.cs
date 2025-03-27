using UnityEngine;

namespace Rougelike2D
{
    public class PlayerAirState : BaseState
    {
        public PlayerAirState(PlayerController controller, AnimatorBrain animatorBrain) : base(controller, animatorBrain)
        {
        }
        public override void EnterState()
        {
            base.EnterState();
            if (playerController.IsJumping)
            {
                animatorBrain.Play(AnimationString.PlayerJump);
            }
            else if (playerController.IsFalling)
            {
                animatorBrain.Play(AnimationString.PlayerFall);
            }
        }
        public override void StateFixedUpdate()
        {
            base.StateFixedUpdate();
            bool isJumpFalling = false;
            playerController.HandleJump();
            playerController.HandleMovement();
            if (playerController.IsJumping)
            {
                animatorBrain.Play(AnimationString.PlayerJumping);
                isJumpFalling = true;
            }
            else if (playerController.IsFalling)
            {
                if (isJumpFalling)
                {
                    animatorBrain.Play(AnimationString.PlayerFall);
                    isJumpFalling = false;
                }
                else
                {
                    animatorBrain.Play(AnimationString.PlayerFalling);
                }
            }
        }
    }
}
