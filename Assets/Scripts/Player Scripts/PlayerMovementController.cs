using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rougelike2D
{
  [RequireComponent(typeof(PlayerController))]
  public class PlayerMovementController : MonoBehaviour
  {
    PlayerController _playerController;
    InputReader _inputReader => _playerController.InputReader;
    PlayerMovementStats _movementStats => _playerController.PlayerMovementStats;
    private CollisionCheck _collisionCheck => _playerController.CollisionCheck;

    private Rigidbody2D _rb => _playerController.RB;

    public bool CanMove = true;

    #region Variables
    public Vector2 MoveDirection { get; private set; }
    public Vector2 Velocity { get; private set; }
    //Player State
    public bool IsMoving => Mathf.Abs(MoveDirection.x) > 0.01f;
    public bool IsFacingRight { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsFalling { get; private set; }
    public bool IsLanding { get; private set; }

    public bool _isJumpFalling { get; private set; }
    public bool IsSliding { get; private set; }
    public bool IsCrouching { get; private set; }
    public bool IsRolling { get; private set; }

    public bool IsGrounded => _collisionCheck.IsGrounded;

    //Player Timers
    List<Timer> timerList;
    public CountDownTimer CoyoteTimer { get; private set; }
    public CountDownTimer RollPressTimer { get; private set; }
    public CountDownTimer RollTimer {get; private set;}
    public CountDownTimer RollCoolDownTimer {get; private set;}
    public CountDownTimer PressedJumpTimer { get; private set; }
    public CountDownTimer LandTimer { get; private set; }

    //Jump
    private bool _isJumpCut;
    private bool _isFallingLand;
 
    private bool ToggleCrouch = true;
    #endregion

    #region UnityActions events
    public UnityAction OnJumpLand = delegate { };
    public UnityAction OnJump = delegate { };
    public UnityAction<bool> OnFall = delegate { };
    public UnityAction<Vector2, float> OnMove = delegate { };
    #endregion

    private void Awake()
    {
      _playerController = GetComponent<PlayerController>();
      _inputReader.Enable();
      SetupTimers();
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
      _inputReader.OnPlayerRoll += RollInput;
    }
    private void Update()
    {
      MoveInput();
      Updatetimer();
      CheckCollision();
      ManageGravity();
      JumpCheck();
      RollCheck();
      //Turn();
    }


    #region Input Handler
    private void MoveInput()
    {
      MoveDirection = _inputReader.Move;
      //Turn();
    }
    private void JumpInput(bool jumpPressed)
    {
      if (jumpPressed && !PressedJumpTimer.IsRunning)
      {
        PressedJumpTimer.StartTimer();
      }
      else
      {
        if (CanJumpCut())
          _isJumpCut = true;
      }
    }
    private void CrouchInput(bool crouchPressed)
    {
      //If we are using toggle crouch, we can just toggle the crouch state
      if (ToggleCrouch)
      {
        if (crouchPressed)
        {
          IsCrouching = !IsCrouching;
        }
        return;
      }
      //If we are not using toggle crouch, we can just set the crouch state to the input
      IsCrouching = crouchPressed;
    }
    private void RollInput()
    {
      if(!RollTimer.IsRunning && !RollCoolDownTimer.IsRunning)
      {
        RollPressTimer.StartTimer();
      }
        
    }
    #endregion

    #region Timers
    private void SetupTimers()
    {
      CoyoteTimer = new CountDownTimer(_movementStats.coyoteTime);
      RollPressTimer = new CountDownTimer(_movementStats.RollInputBufferTime);
      RollTimer = new CountDownTimer(_movementStats.RollTime);
      RollCoolDownTimer = new CountDownTimer(_movementStats.RollCoolDown);
      PressedJumpTimer = new CountDownTimer(_movementStats.jumpInputBufferTime);
      LandTimer = new CountDownTimer(_movementStats.landRecoverTime);

      timerList = new(6) { CoyoteTimer, RollPressTimer, PressedJumpTimer, LandTimer, RollCoolDownTimer, RollTimer };

      RollTimer.OnStopTimer += () => RollCoolDownTimer.StartTimer();
    }
    private void Updatetimer()
    {
      foreach (Timer timer in timerList)
      {
        timer.Tick(Time.deltaTime);
      }
    }
    #endregion

    #region Collision Check
    private void CheckCollision()
    {
      if (!IsJumping)
      {
        if (!_collisionCheck.IsGrounded)
        {
          CoyoteTimer.StartTimer();
        }
      }
    }
    #endregion

    #region Manage Gravity
    private void ManageGravity()
    {
      if (_rb.linearVelocity.y < 0 && MoveDirection.y < 0)
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

    public void Run()
    {
      Turn();
      //Calculate the direction we want to move in and our desired velocity
      float targetSpeed = MoveDirection.x * _movementStats.runMaxSpeed;
      //We can reduce are control using Lerp() this smooths changes to are direction and speed
      targetSpeed = Mathf.Lerp(_rb.linearVelocity.x, targetSpeed, 1);

      float accelRate;

      if (IsGrounded || CoyoteTimer.IsRunning)
        accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _movementStats.runAccelAmount : _movementStats.runDeccelAmount;
      else
        accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _movementStats.runAccelAmount * _movementStats.accelInAir : _movementStats.runDeccelAmount * _movementStats.deccelInAir;
      //Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
      if ((IsJumping || _isJumpFalling) && Mathf.Abs(_rb.linearVelocity.y) < _movementStats.jumpHangTimeThreshold)
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

      Velocity = _rb.linearVelocity;
      OnMove?.Invoke(_rb.linearVelocity, MoveDirection.x);
    }

    private void Turn()
    {
      if (MoveDirection.x > 0 && !IsFacingRight)
      {
        IsFacingRight = true;
        transform.localScale = Vector3.one;
      }
      else if (MoveDirection.x < 0 && IsFacingRight)
      {
        IsFacingRight = false;
        transform.localScale = new Vector3(-1, 1, 1);
      }
    }
    public void StopMovement()
    {
      MoveDirection = Vector2.zero;
      Run();
    }
    #endregion

    #region Check Jump Conditions
    public void JumpCheck()
    {
      IsFalling = _rb.linearVelocity.y < -0.02f && !IsGrounded;
      if (IsFalling)
      {
        IsJumping = false;
        _isFallingLand = true;
      }
      OnFall?.Invoke(IsFalling);
      if (IsJumping && _rb.linearVelocity.y < 0)
      {
        IsJumping = false;
        _isJumpFalling = true;
      }
      if ((IsGrounded || CoyoteTimer.IsRunning) && !IsJumping)
      {
        _isJumpCut = false;
        if (!IsJumping)
          _isJumpFalling = false;
      }
      if (CanJump() && PressedJumpTimer.IsRunning)
      {
        IsJumping = true;
        _isJumpCut = false;
        _isJumpFalling = false;
        Jump();
      }
      if (_isFallingLand && IsGrounded)
      {
        _isFallingLand = false;
        IsFalling = false;
        LandTimer.StartTimer();
      }
    }
    private bool CanJump()
    {
      return (IsGrounded || CoyoteTimer.IsRunning) && !IsJumping;
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
      PressedJumpTimer.StopTimer();
      CoyoteTimer.StopTimer();

      //We increase the force applied if we are falling
      float force = _movementStats.jumpForce;
      if (_rb.linearVelocity.y < 0)
        force -= _rb.linearVelocity.y;

      _rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
      OnJump?.Invoke();
    }

    #endregion

    #region Roll
    public void RollCheck()
    {
      if(RollPressTimer.IsRunning)
      {
        RollTimer.StartTimer();
      }
    }
    public void Roll()
    {
      RollPressTimer.StopTimer();
      CoyoteTimer.StopTimer();
      var currentGravity = _rb.gravityScale;
      SetGravityScale(0);
      float dir = 0;
      if (MoveDirection.x != 0)
        dir = MoveDirection.normalized.x;
      else
        dir = IsFacingRight ? 1 : -1;

      float force = _movementStats.RollAmount;
      if (_rb.linearVelocity.normalized.x == dir)
      {
        //force -= _rb.linearVelocity.x;
      }
      _rb.AddForce(Vector2.right * dir * force, ForceMode2D.Impulse);
    }
    #endregion
  }
}
