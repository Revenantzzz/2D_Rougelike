using System;
using NUnit.Framework;
using UnityEngine;

namespace Rougelike2D
{
    public class PlayerMovementController : MonoBehaviour
    {
      [SerializeField] InputReader _inputReader;
      [SerializeField] PlayerData _data;
      private Rigidbody2D _rb;
      private CollisionCheck _collisionCheck;

      #region Variables
      private Vector2 _moveDirection;
      //Player State
      public bool IsMoving { get; private set; }
      public bool IsFacingRight { get; private set; }
      public bool IsJumping { get; private set; }
      public bool IsWallJumping { get; private set; }
      public bool IsFalling { get; private set; }
      public bool IsSliding { get; private set; }
      //Player Timers
      public float LastOnGroundTime { get; private set; }
      public float LastOnWallTime { get; private set; }
      public float LastOnWallRightTime { get; private set; }
      public float LastOnWallLeftTime { get; private set; }

      //Jump
      private bool _isJumpCut;
      private bool _isJumpFalling;

      //Wall Jump
      private float _wallJumpStartTime;
      private int _lastWallJumpDir;

      public float LastPressedJumpTime { get; private set; }

      #endregion
      private void Awake() 
      {
        _inputReader.Enable();
        _rb = GetComponent<Rigidbody2D>();
        _collisionCheck = GetComponentInChildren<CollisionCheck>();
      }
      private void Start() 
      {
        IsFacingRight = true; 
        InputEvent();
      }
      private void InputEvent()
      {
        _inputReader.OnPlayerJump += JumpInput;
      }
      private void Update() 
      {
        UpdateTimers();
        CheckCollision();
        ManageGravity();
        JumpCheck();
      }
      private void FixedUpdate() 
      {
        Run(1);
        MoveInput();
      }

      #region Input Handler
      private void MoveInput()
      {
        _moveDirection = _inputReader.Move;
        Turn();
      } 
      private void JumpInput(bool jumpPressed)
      {
        if (jumpPressed)
        {
          LastPressedJumpTime = _data.jumpInputBufferTime;
        }
        else
        {
          if(CanJumpCut() || CanWallJumpCut())
            _isJumpCut = true;
        }
      }
      #endregion
      
      #region Timers
      private void UpdateTimers()
      {
        LastOnGroundTime -= Time.deltaTime;
        LastOnWallTime -= Time.deltaTime;
        LastOnWallRightTime -= Time.deltaTime;
        LastOnWallLeftTime -= Time.deltaTime;

        LastPressedJumpTime -= Time.deltaTime;
      }
      #endregion

      #region Collision Check
      private void CheckCollision()
      {
        if (!IsJumping)
		    {
          if(_collisionCheck.IsGrounded)
          {
            LastOnGroundTime = _data.coyoteTime;
          }
          if(_collisionCheck.IsOnWall)
          {
            LastOnWallTime = _data.coyoteTime;
            if(!IsFacingRight)
            {
              LastOnWallRightTime = _data.coyoteTime;
              _lastWallJumpDir = 1;
            }
            else
            {
              LastOnWallLeftTime = _data.coyoteTime;
              _lastWallJumpDir = -1;
            }
          }
          LastOnWallTime = Mathf.Max(LastOnWallRightTime, LastOnWallLeftTime);
        }
      }
      #endregion

      #region Manage Gravity
      private void ManageGravity()
      {
        if (IsSliding)
        {
          SetGravityScale(0);
          return;
        }
        if (_rb.linearVelocity.y < 0 && _moveDirection.y < 0)
        {
          //Much higher gravity if holding down
          SetGravityScale(_data.gravityScale * _data.fastFallGravityMult);
          //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
          _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, Mathf.Max(_rb.linearVelocity.y, -_data.maxFastFallSpeed));
          return;
        }
        if (_isJumpCut)
        {
          //Higher gravity if jump button released
          SetGravityScale(_data.gravityScale * _data.jumpCutGravityMult);
          _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, Mathf.Max(_rb.linearVelocity.y, -_data.maxFallSpeed));
          return;
        }
        if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(_rb.linearVelocity.y) < _data.jumpHangTimeThreshold)
        {
          SetGravityScale(_data.gravityScale * _data.jumpHangGravityMult);
          return;
        }
        if (_rb.linearVelocity.y < 0)
        {
          //Higher gravity if falling
          SetGravityScale(_data.gravityScale * _data.fallGravityMult);
          //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
          _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, Mathf.Max(_rb.linearVelocity.y, -_data.maxFallSpeed));
          return;
        }
        //Default gravity if standing on a platform or moving upwards
        SetGravityScale(_data.gravityScale);
      }
      private void SetGravityScale(float gravityScale)
      {
        _rb.gravityScale = gravityScale;
      }  
      #endregion

      #region Movement
      private void Run(float lerpAmount)
      {
        //Calculate the direction we want to move in and our desired velocity
        float targetSpeed = _moveDirection.x * _data.runMaxSpeed;
        //We can reduce are control using Lerp() this smooths changes to are direction and speed
        targetSpeed = Mathf.Lerp(_rb.linearVelocity.x, targetSpeed, lerpAmount);

        float accelRate;

        if (LastOnGroundTime > 0)
          accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ?_data.runAccelAmount :_data.runDeccelAmount;
        else
          accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ?_data.runAccelAmount *_data.accelInAir :_data.runDeccelAmount *_data.deccelInAir;
        //Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
        if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(_rb.linearVelocity.y) < _data.jumpHangTimeThreshold)
        {
          accelRate *= _data.jumpHangAccelerationMult;
          targetSpeed *= _data.jumpHangMaxSpeedMult;
        }
        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed -_rb.linearVelocity.x;
        //Calculate force along x-axis to apply to thr player
        float movement = speedDif * accelRate;
        //Convert this to a vector and apply to rigidbody
       _rb.AddForce(movement * Vector2.right, ForceMode2D.Force);

       IsMoving = Mathf.Abs(_rb.linearVelocity.x) > 0.01f;
      }

      private void Turn()
      {
        if (_moveDirection.x > 0 && !IsFacingRight)
        {
          IsFacingRight = true;
          transform.Rotate(0, 180, 0);
        }
        else if (_moveDirection.x < 0 && IsFacingRight)
        {
          IsFacingRight = false;
          transform.Rotate(0, 180, 0);
        }
      }
      #endregion

      #region Jump

        #region Check Jump Conditions
        private void JumpCheck()
        {
          IsFalling = _rb.linearVelocity.y < 0;
          if (IsJumping && _rb.linearVelocity.y < 0)
          {
            IsJumping = false;
            if(!IsWallJumping)
              _isJumpFalling = true;
          }
          if (IsWallJumping && Time.time - _wallJumpStartTime > _data.wallJumpTime)
          {
            IsWallJumping = false;
          }
          if (LastOnGroundTime > 0 && !IsJumping && !IsWallJumping)
          {
            _isJumpCut = false;
            if(!IsJumping)
              _isJumpFalling = false;
          }
          if (CanJump() && LastPressedJumpTime > 0)
          {
            IsJumping = true;
            IsWallJumping = false;
            _isJumpCut = false;
            _isJumpFalling = false;
            Jump();
          }
          if (CanWallJump() && LastPressedJumpTime > 0)
          {
            IsWallJumping = true;
            IsJumping = false;
            _isJumpCut = false;
            _isJumpFalling = false;

            _wallJumpStartTime = Time.time;
            _lastWallJumpDir = (LastOnWallRightTime > 0) ? -1 : 1;

            WallJump(_lastWallJumpDir);
          }
        }
        private bool CanJump()
        {
        return LastOnGroundTime > 0 && !IsJumping;
        }
        private bool CanWallJump()
        {  
          if(LastPressedJumpTime <= 0)
            return false;
          if(LastOnWallTime <= 0)
            return false;
          if(LastOnGroundTime > 0)
            return false;
          if(IsWallJumping && (LastOnWallRightTime < 0 || _lastWallJumpDir != 1) && (LastOnWallLeftTime < 0 || _lastWallJumpDir != -1))
          return false;

          return true;
        }
        private bool CanJumpCut()
        {
          return IsJumping && _rb.linearVelocity.y > 0;
        }
        private bool CanWallJumpCut()
        {
          return IsWallJumping && _rb.linearVelocity.y > 0;
        } 
        #endregion
      private void Jump()
      {
        //Ensures we can't call Jump multiple times from one press
		    LastPressedJumpTime = 0;
		    LastOnGroundTime = 0;

        //We increase the force applied if we are falling
        float force = _data.jumpForce;
        if (_rb.linearVelocity.y < 0)
          force -= _rb.linearVelocity.y;

        _rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
      } 
      private void WallJump(int dir)
      {
        //Ensures we can't call Wall Jump multiple times from one press
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;
        LastOnWallRightTime = 0;
        LastOnWallLeftTime = 0;

        Vector2 force = new Vector2(_data.wallJumpForce.x, _data.wallJumpForce.y);
        force.x *= dir; //apply force in opposite direction of wall

        if (Mathf.Sign(_rb.linearVelocity.x) != Mathf.Sign(force.x))
          force.x -= _rb.linearVelocity.x;

        if (_rb.linearVelocity.y < 0) //checks whether player is falling, if so we subtract the velocity.y (counteracting force of gravity). This ensures the player always reaches our desired jump force or greater
          force.y -= _rb.linearVelocity.y;

        _rb.AddForce(force, ForceMode2D.Impulse);
      }
      #endregion
  }
}
