using UnityEngine;

namespace Rougelike2D
{
    public interface IRollMovement
    {
        public abstract void HandleRoll(float dir);
        public abstract void RollCheck();
    }
    public abstract class BaseRollMovement : IRollMovement
    {
        protected Rigidbody2D _rb;
        public BaseRollMovement(Rigidbody2D rb)
        {
            _rb = rb;
        }
        public abstract void HandleRoll(float dir);
        public abstract void RollCheck();
    }

}
