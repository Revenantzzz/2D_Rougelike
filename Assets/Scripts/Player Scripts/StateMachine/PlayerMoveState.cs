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
             if(playerController.IsMoving)
            {
                animatorBrain.Play(AnimationString.PlayerToRun);
            }  
            else
            {
                animatorBrain.Play(AnimationString.PlayerIdle);
            }
        }
        public override void StateUpdate()
        {
            base.StateUpdate();  
        }
        public override void StateFixedUpdate()
        {
            base.StateFixedUpdate();
            playerController.Run();
            if(playerController.IsMoving)
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
