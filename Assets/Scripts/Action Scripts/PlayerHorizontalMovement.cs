using UnityEngine;

namespace Rougelike2D
{
    public class PlayerHorizontalMovement : BaseHorizontalMovement
    {
        private PlayerMovementStats _movementStats;
        public PlayerHorizontalMovement(Transform transform, Rigidbody2D rb, PlayerMovementStats movementStats) : base(transform, rb)
        {
            this._movementStats = movementStats;
        }
        public override void HandleHorizontalMovement(float _moveDirectionX)
        {
            Turn(_moveDirectionX);
            //Calculate the direction we want to move in and our desired velocity
            float targetSpeed = _moveDirectionX * _movementStats.runMaxSpeed;
            //We can reduce are control using Lerp() this smooths changes to are direction and speed
            targetSpeed = Mathf.Lerp(_rb.linearVelocity.x, targetSpeed, 1);

            float accelRate;

            if (IsGrounded)
                accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _movementStats.runAccelAmount : _movementStats.runDeccelAmount;
            else
                accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _movementStats.runAccelAmount * _movementStats.accelInAir : _movementStats.runDeccelAmount * _movementStats.deccelInAir;
            //Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
            if ((IsJumping) && Mathf.Abs(_rb.linearVelocity.y) < _movementStats.jumpHangTimeThreshold)
            {
                accelRate *= _movementStats.jumpHangAccelerationMult;
                targetSpeed *= _movementStats.jumpHangMaxSpeedMult;
            }
            //Calculate difference between current velocity and desired velocity
            float speedDif = targetSpeed - _rb.linearVelocity.x;
            //Calculate force along x-axis to apply to thr player
            float movement = speedDif * accelRate;
            //Convert this to a vector and apply to rigidbody
            _rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
        }
    }
}
