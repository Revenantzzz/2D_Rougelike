using Unity.Collections;
using UnityEngine;

namespace Rougelike2D
{
    public class PlayerAttackState : BaseState
    {
        public PlayerAttackState(PlayerController controller, AnimatorBrain animatorBrain) : base(controller, animatorBrain)
        {
        }

        public override void EnterState()
        {
            base.EnterState();
            playerController.HorizontalMovement.StopMovement();
            animatorBrain.SetFloatValue("Attack Count", playerController.AttackCount);
            animatorBrain.Play(AnimationString.PlayerAttack);
        }
    }
}
