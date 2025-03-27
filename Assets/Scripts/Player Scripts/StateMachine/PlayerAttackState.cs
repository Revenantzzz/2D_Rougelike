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
            playerController.StopMovement();
            animatorBrain.SetFloatValue("Attack Count", playerController.AttackCounter);
            animatorBrain.Play(AnimationString.PlayerAttack);
        }
    }
}
