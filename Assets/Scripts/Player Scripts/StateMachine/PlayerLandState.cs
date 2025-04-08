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
            if(playerController.IsFastFalling)
            {
                Debug.Log("land hard");
                animatorBrain.Play("Player_LandHard");
            }
            else
            {
                animatorBrain.Play(AnimationString.PlayerLand);
            }     
            playerController.HorizontalMovement.StopMovement();
        }
        public override void StateUpdate()
        {
            base.StateUpdate();
            playerController.HorizontalMovement.StopMovement();
        }
    }
}
