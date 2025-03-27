using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Accessibility;

namespace Rougelike2D
{
    public abstract class BaseState : IState
    {
        protected PlayerController playerController;
        protected AnimatorBrain animatorBrain;

        public BaseState(PlayerController controller, AnimatorBrain animatorBrain)
        {
            this.playerController = controller;
            this.animatorBrain = animatorBrain;
        }
        public virtual void EnterState()
        {
            // 
        }

        public virtual void ExitState()
        {
            // 
        }

        public virtual void StateFixedUpdate()
        {
            // 
        }

        public virtual void StateUpdate()
        {
            // 
        }
    }
}
