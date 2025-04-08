using System.Collections.Generic;
using UnityEngine;

namespace Rougelike2D
{
    public class PlayerJump : BaseJumpMovement
    {
        private PlayerController _playerController;
        private PlayerMovementStats _movementStats;
        public PlayerJump(PlayerController controller, Rigidbody2D rb, PlayerMovementStats movementStats) 
        : base( rb)
        {
            _playerController = controller;
            _movementStats = movementStats;
        }

        public void HandleInput(bool jumpPressed)
        {
            if (jumpPressed && !_playerController.PressedJumpTimer.IsRunning)
            {
                _playerController.PressedJumpTimer.StartTimer();
            }
            else
            {
                if (CanJumpCut())
                    IsJumpCut = true;
            }
        }
        public override void HandleJump()
        {
            //Ensures we can't call Jump multiple times from one press
            _playerController.PressedJumpTimer.StopTimer();
            _playerController.CoyoteTimer.StopTimer();

            //We increase the force applied if we are falling
            float force = _movementStats.jumpForce;
            if (_rb.linearVelocity.y < 0)
                force -= _rb.linearVelocity.y;

            _rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        }
        bool _isFallingLand = false;
        public override void JumpCheck()
        {
            IsFalling = _rb.linearVelocity.y < -0.02f && !_playerController.IsGrounded;
            if (IsFalling)
            {
                IsJumping = false;
                _isFallingLand = true;
            }
            if (IsJumping && _rb.linearVelocity.y < 0)
            {
                IsJumping = false;
                IsJumpFalling = true;
            }
            if (_playerController.IsGrounded && !IsJumping)
            {
                IsJumpCut = false;
                if (!IsJumping)
                    IsJumpFalling = false;
            }
            if (CanJump() && _playerController.PressedJumpTimer.IsRunning)
            {
                IsJumping = true;
                IsJumpCut = false;
                IsJumpFalling = false;
                HandleJump();
            }
            if (_isFallingLand && _playerController.IsGrounded && ! _playerController.LandTimer.IsRunning)
            {
                IsJumping = false;
                IsJumpFalling = false;
                _isFallingLand = false;
                IsFalling = false;

                if (IsFastFalling) _playerController.LandTimer.Reset(_movementStats.fastLandRecoverTime);
                else _playerController.LandTimer.Reset(_movementStats.landRecoverTime);
                IsFastFalling = false;
                _playerController.LandTimer.StartTimer();
            }
        }
        private bool CanJump()
        {
            return (_playerController.IsGrounded || _playerController.CoyoteTimer.IsRunning) && !IsJumping && !IsFalling;
        }
        private bool CanJumpCut()
        {
            return IsJumping && _rb.linearVelocity.y > 0;
        }
    }
}
