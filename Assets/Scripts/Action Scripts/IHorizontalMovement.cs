using System;
using UnityEngine;

namespace Rougelike2D
{
    public interface IHorizontalMovement
    {
        public void HandleHorizontalMovement(float directionX);
        public void StopMovement() => HandleHorizontalMovement(0f);
        public bool IsGrounded { get; set;}
        public bool IsJumping { get; }
    }
    public abstract class BaseHorizontalMovement  : IHorizontalMovement
    {
        protected Rigidbody2D _rb;
        protected Transform _transform;
        protected bool _isFacingRight = true;
        public bool IsGrounded { get; set;}
        public bool IsJumping { get; protected set; }
        public BaseHorizontalMovement(Transform transform, Rigidbody2D rb)
        {
            _transform = transform;
            _rb = rb;
        }

        public abstract void HandleHorizontalMovement(float directionX);
        public void StopMovement()
        {
            HandleHorizontalMovement(0f);
        }
        public void Turn(float moveDirectionX)
        {
            if (moveDirectionX > 0 && !_isFacingRight)
            {
                _transform.localScale = new Vector3(-_transform.localScale.x, _transform.localScale.y, _transform.localScale.z);
                _isFacingRight = true;
            }
            else if (moveDirectionX < 0 && _isFacingRight)
            {
                _transform.localScale = new Vector3(-_transform.localScale.x, _transform.localScale.y, _transform.localScale.z);
                _isFacingRight = false;
            }
        }
    }
}
