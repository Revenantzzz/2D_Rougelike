using System;
using System.Collections;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Rougelike2D
{
  [RequireComponent(typeof(PlayerController))]
    public class PlayerMovementController : MonoBehaviour
    {
      PlayerController _playerController;
      InputReader _inputReader =>_playerController.InputReader;
      PlayerMovementStats _movementStats => _playerController.PlayerMovementStats;
      private CollisionCheck _collisionCheck => _playerController.CollisionCheck;

      private Rigidbody2D _rb;

      public bool CanMove = true;

      #region Variables
      private Vector2 _moveDirection;
      //Player State
      public bool IsMoving { get; private set; }
      public bool IsFacingRight { get; private set; }
      public bool IsJumping { get; private set; }
      public bool IsFalling { get; private set; }
      public bool IsLanding { get; private set; }
      
      public bool _isJumpFalling{ get; private set; }
      public bool IsSliding { get; private set; }
      public bool IsCrouching { get; private set; }
      public bool IsDashing {get; private set;}

      public bool IsGrounded => LastOnGroundTime > 0;
      public bool IsOnWall => LastOnWallTime > 0;

      //Player Timers
      public float LastOnGroundTime { get; private set; }
      public float LastOnWallTime { get; private set; }
      public float LastOnWallRightTime { get; private set; }
      public float LastOnWallLeftTime { get; private set; }
      public float LastPressedDashTime {get; private set;}
      public float LastPressedJumpTime { get; private set; }
      
      //Jump
      private bool _isJumpCut;
      private bool _isFallingLand;
      //Dash
      private bool _dashRefilling;
      private float _dashesLeft;
      private bool _isDashAttacking;
      private Vector2 _lastDashDir;
      
      private bool ToggleCrouch = true;
      #endregion

      #region UnityActions events
      public UnityAction OnJumpLand = delegate{};
      public UnityAction OnJump = delegate{};
      public UnityAction<bool> OnFall = delegate{};
      public UnityAction<Vector2, float> OnMove = delegate{};
      #endregion

      private void Awake() 
      {
        _playerController = GetComponent<PlayerController>();
        _inputReader.Enable();
        _rb = GetComponent<Rigidbody2D>();
      }
      private void Start() 
      {
        IsFacingRight = true; 
        InputEvent();
        Application.targetFrameRate = 60;
      }
      private void InputEvent()
      {
        _inputReader.OnPlayerJump += JumpInput;
        _inputReader.OnPlayerDash += DashInput;
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
        MoveInput();
        if (!IsDashing)
		    {
				  Run(1);
		    }
        else if (_isDashAttacking)
        {
          Run(_movementStats.dashEndRunLerp);
        }
      }

      #region Input Handler
      private void MoveInput()
      {
        if(!CanMove)
        {
          _moveDirection = Vector2.zero;
          return;
        }
        _moveDirection = _inputReader.Move;
        Turn();
      } 
      private void JumpInput(bool jumpPressed)
      {
        if(!CanMove)
        {
          return;
        }
        if (jumpPressed)
        {
          LastPressedJumpTime = _movementStats.jumpInputBufferTime;
        }
        else
        {
          if(CanJumpCut())
            _isJumpCut = true;
        }
      }
      private void CrouchInput(bool crouchPressed)
      {
        if(!CanMove)
        {
          return;
        }
        //If we are using toggle crouch, we can just toggle the crouch state
        if(ToggleCrouch)
        {
          if(crouchPressed)
          {
            IsCrouching = !IsCrouching;
          }
          return;
        }
        //If we are not using toggle crouch, we can just set the crouch state to the input
        IsCrouching = crouchPressed;
      }
      private void DashInput()
      {
        LastPressedDashTime = _movementStats.dashInputBufferTime;
      }
      #endregion
      
      #region Timers
      private void UpdateTimers()
      {
        if(LastOnGroundTime >= 0)LastOnGroundTime -= Time.deltaTime;
        if(LastOnWallTime >= 0) LastOnWallTime -= Time.deltaTime;
        if(LastOnWallRightTime >= 0) LastOnWallRightTime -= Time.deltaTime;
        if(LastOnWallLeftTime >= 0)LastOnWallLeftTime -= Time.deltaTime;

        if(LastPressedJumpTime >= 0) LastPressedJumpTime -= Time.deltaTime;
        if(LastPressedDashTime >= 0) LastPressedDashTime -= Time.deltaTime;
      }
      #endregion

      #region Collision Check
      private void CheckCollision()
      {
        if (!IsJumping)
		    {
          if(_collisionCheck.IsGrounded)
          {
            LastOnGroundTime = _movementStats.coyoteTime;
          }
          // if(_collisionCheck.IsOnWall)
          // {
          //   LastOnWallTime = _movementStats.coyoteTime;
          //   if(!IsFacingRight)
          //   {
          //     LastOnWallRightTime = _movementStats.coyoteTime;
          //   }
          //   else
          //   {
          //     LastOnWallLeftTime = _movementStats.coyoteTime;
          //   }
          // }
          // LastOnWallTime = Mathf.Max(LastOnWallRightTime, LastOnWallLeftTime);
        }
      }
      #endregion

      #region Manage Gravity
      private void ManageGravity()
      {
        if (_rb.linearVelocity.y < 0 && _moveDirection.y < 0)
        {
          //Much higher gravity if holding down
          SetGravityScale(_movementStats.gravityScale * _movementStats.fastFallGravityMult);
          //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
          _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, Mathf.Max(_rb.linearVelocity.y, -_movementStats.maxFastFallSpeed));
          return;
        }
        if (_isJumpCut)
        {
          //Higher gravity if jump button released
          SetGravityScale(_movementStats.gravityScale * _movementStats.jumpCutGravityMult);
          _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, Mathf.Max(_rb.linearVelocity.y, -_movementStats.maxFallSpeed));
          return;
        }
        if ((IsJumping || _isJumpFalling) && Mathf.Abs(_rb.linearVelocity.y) < _movementStats.jumpHangTimeThreshold)
        {
          SetGravityScale(_movementStats.gravityScale * _movementStats.jumpHangGravityMult);
          return;
        }
        if (_rb.linearVelocity.y < 0)
        {
          //Higher gravity if falling
          SetGravityScale(_movementStats.gravityScale * _movementStats.fallGravityMult);
          //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
          _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, Mathf.Max(_rb.linearVelocity.y, -_movementStats.maxFallSpeed));
          return;
        }
        //Default gravity if standing on a platform or moving upwards
        SetGravityScale(_movementStats.gravityScale);
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
        float targetSpeed = _moveDirection.x * _movementStats.runMaxSpeed;
        //We can reduce are control using Lerp() this smooths changes to are direction and speed
        targetSpeed = Mathf.Lerp(_rb.linearVelocity.x, targetSpeed, lerpAmount);

        float accelRate;

        if (LastOnGroundTime > 0)
          accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ?_movementStats.runAccelAmount :_movementStats.runDeccelAmount;
        else
          accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ?_movementStats.runAccelAmount *_movementStats.accelInAir :_movementStats.runDeccelAmount *_movementStats.deccelInAir;
        //Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
        if ((IsJumping || _isJumpFalling) && Mathf.Abs(_rb.linearVelocity.y) < _movementStats.jumpHangTimeThreshold)
        {
          accelRate *= _movementStats.jumpHangAccelerationMult;
          targetSpeed *= _movementStats.jumpHangMaxSpeedMult;
        }
        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed -_rb.linearVelocity.x;
        //Calculate force along x-axis to apply to thr player
        float movement = speedDif * accelRate;
        //Convert this to a vector and apply to rigidbody
       _rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
      
       OnMove?.Invoke(_rb.linearVelocity, _moveDirection.x);
      }

      private void Turn()
      {
        if (_moveDirection.x > 0 && !IsFacingRight)
        {
          IsFacingRight = true;
          transform.localScale = Vector3.one;
        }
        else if (_moveDirection.x < 0 && IsFacingRight)
        {
          IsFacingRight = false;
          transform.localScale = new Vector3(-1,1,1);
        }
      }
      #endregion

      #region Check Jump Conditions
      private void JumpCheck()
      {
        IsFalling = _rb.linearVelocity.y < -0.02f && LastOnGroundTime < 0;
        if(IsFalling)
        {
          _isFallingLand = true;
          Debug.Log(_rb.linearVelocityY);
        }
        OnFall?.Invoke(IsFalling);
        if (IsJumping && _rb.linearVelocity.y < 0)
        {
          IsJumping = false;
          _isJumpFalling = true;
        }
        if (LastOnGroundTime > 0 && !IsJumping)
        {
          _isJumpCut = false;
          if(!IsJumping)
            _isJumpFalling = false;
        }
        if (CanJump() && LastPressedJumpTime > 0)
        {
          IsJumping = true;
          _isJumpCut = false;
          _isJumpFalling = false;
          Jump();
        }
        if(_isFallingLand && LastOnGroundTime > 0)
        {
          _isFallingLand = false;
          StartCoroutine(LandRecover());
        }
      }
      private bool CanJump()
      {
        return LastOnGroundTime > 0 && !IsJumping;
      }
      private bool CanJumpCut()
      {
        return IsJumping && _rb.linearVelocity.y > 0;
      }
      #endregion
      
      #region Jump
      private void Jump()
      {
        //Ensures we can't call Jump multiple times from one press
		    LastPressedJumpTime = 0;
		    LastOnGroundTime = 0;

        //We increase the force applied if we are falling
        float force = _movementStats.jumpForce;
        if (_rb.linearVelocity.y < 0)
          force -= _rb.linearVelocity.y;

        _rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        OnJump?.Invoke();
      } 

      private IEnumerator LandRecover()
      {
          OnJumpLand?.Invoke();
          CanMove = false;
          yield return new WaitForSeconds(0.5f);
          CanMove = true; 
      }
      #endregion

      private void Dash()
      {
        
      }
      
  }
}
