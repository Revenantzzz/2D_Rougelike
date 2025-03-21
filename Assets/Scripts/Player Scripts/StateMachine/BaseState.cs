using UnityEngine;

namespace Rougelike2D
{
    public abstract class BaseState : IState
    {
        private PlayerController playerController;
        private Animator animator;

        public BaseState(PlayerController controller, Animator animator)
        {
            this.playerController = controller;
            this.animator = animator;
        }
        public void EnterState()
        {
            // 
        }

        public void ExitState()
        {
            // 
        }

        public void StateFixedUpdate()
        {
            // 
        }

        public void StateUpdate()
        {
            // 
        }
    }
}
