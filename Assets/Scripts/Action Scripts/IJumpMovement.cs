using UnityEngine;

namespace Rougelike2D
{
    public interface IJumpMovement
    {
        public bool IsJumping {get;}
        public bool IsFalling {get;}
        public bool IsJumpCut {get;}
        public bool IsJumpFalling {get;}
        public bool IsFastFalling {get; set;}

        public void HandleJump();
        public void JumpCheck();    
    }
    public abstract class BaseJumpMovement : IJumpMovement
    {
        protected Rigidbody2D _rb;
        public bool IsJumping { get; protected set; }
        public bool IsFalling {get; protected set;}
        public bool IsJumpCut { get; protected set; }
        public bool IsJumpFalling { get; protected set; }
        public bool IsFastFalling { get;set; }

        public BaseJumpMovement( Rigidbody2D rb)
        {
            _rb = rb;
        }
        public abstract void HandleJump();
        public abstract void JumpCheck();
    }
}
