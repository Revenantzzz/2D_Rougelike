using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Rougelike2D
{
  public class PlayerController : MonoBehaviour
  {
    #region Components
    public static PlayerController Instance;
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private PlayerMovementStats _movementStats;
    [SerializeField] private PlayerCombatStats _combatSatats;
    private CollisionCheck _collisionCheck;
    private Rigidbody2D _rb;
    PlayerAnimationManager playerAnimationManager;
    StateMachine stateMachine;

    #endregion

    #region Variables
    public Vector2 MoveDirection { get; private set; }
    public Vector2 Velocity { get; private set; }
    private bool _isGrounded => _collisionCheck.IsGrounded;
    private bool _isOnLedge => _collisionCheck.IsOnLedge;
    //Player State
    public bool IsMoving => Mathf.Abs(MoveDirection.x) > 0.01f;
    public bool IsFacingRight { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsFalling { get; private set; }
    //Jump
    public bool IsFastFalling { get; private set; }
    private bool _isJumpCut;
    private bool _isFallingLand;
    private bool _isJumpFalling;
    //combat
    public int AttackCount { get; private set; }
    public bool IsAttacking { get; private set; }
    public bool CanAttack { get; private set; }
    public bool IsBlocking { get; private set; }
    public bool IsComboAttack;
    //Player Timers
    List<Timer> timerList;
    private CountDownTimer _coyoteTimer;
    private CountDownTimer _rollPressTimer;
    private CountDownTimer _rollTimer;
    private CountDownTimer _rollCoolDownTimer;
    private CountDownTimer _pressedJumpTimer;
    private CountDownTimer _landTimer;
    private CountDownTimer _ledgeClimbTimer;
    private CountDownTimer _ledgeClimbRecoverTimer;
    private CountDownTimer _attackTimer;
    private CountDownTimer _attackCoolDownTimer;
    private CountDownTimer _comboTimer;

    #endregion

    private void Awake()
    {
      Singelton();
      _collisionCheck = GetComponentInChildren<CollisionCheck>();
      playerAnimationManager = GetComponentInChildren<PlayerAnimationManager>();
      _rb = GetComponent<Rigidbody2D>();
      HandleStateMachine();
      _inputReader.Enable();
      SetupTimers();
    }
    private void Start()
    {
      IsFacingRight = true;
      InputEvent();
      Application.targetFrameRate = 60;
    }
    private void Singelton()
    {
      //Singleton
      if (Instance == null) Instance = this;
      else Destroy(this);
    }
    private void InputEvent()
    {
      _inputReader.OnPlayerJump += JumpInput;
      _inputReader.OnPlayerRoll += RollInput;
      _inputReader.OnPlayerAttack += AttackInput;
      _inputReader.OnPlayerBlock += BlockInput;
    }
    private void Update()
    {
      MoveInput();
      Updatetimer();
      CheckCollision();
      ManageGravity();
      JumpCheck();
      RollCheck();
      HandleLedgeClimb();
      stateMachine.Update();
    }
    void FixedUpdate()
    {
      stateMachine.FixedUpdate();
    }

    #region Input Handler
    private void MoveInput()
    {
      MoveDirection = _inputReader.Move;
    }
    private void JumpInput(bool jumpPressed)
    {
      if (jumpPressed && !_pressedJumpTimer.IsRunning)
      {
        _pressedJumpTimer.StartTimer();
      }
      else
      {
        if (CanJumpCut())
          _isJumpCut = true;
      }
    }
    private void RollInput()
    {
      if (!_rollTimer.IsRunning && !_rollCoolDownTimer.IsRunning)
      {
        _rollPressTimer.StartTimer();
      }
    }
    private void AttackInput(bool attack)
    {
      if (attack && !_attackTimer.IsRunning && !_attackCoolDownTimer.IsRunning)
      {
        _attackTimer.StartTimer();
      }
    }
    private void BlockInput(bool block)
    {
      IsBlocking = block;
    }
    #endregion

    #region Timers
    private void SetupTimers()
    {
      _coyoteTimer = new CountDownTimer(_movementStats.coyoteTime);
      _rollPressTimer = new CountDownTimer(_movementStats.RollInputBufferTime);
      _rollTimer = new CountDownTimer(_movementStats.RollTime);
      _rollCoolDownTimer = new CountDownTimer(_movementStats.RollCoolDown);
      _pressedJumpTimer = new CountDownTimer(_movementStats.jumpInputBufferTime);
      _landTimer = new CountDownTimer(_movementStats.landRecoverTime);
      _ledgeClimbTimer = new CountDownTimer(_movementStats.LedgeClimbTime);
      _ledgeClimbRecoverTimer = new CountDownTimer(_movementStats.LedgeClimbRecover);
      _attackTimer = new CountDownTimer(_combatSatats.AttackTime);
      _attackCoolDownTimer = new CountDownTimer(_combatSatats.AttackCoolDown);
      _comboTimer = new CountDownTimer(3f);

      timerList = new() { _coyoteTimer, _rollPressTimer, _pressedJumpTimer, _landTimer, _rollCoolDownTimer, _rollTimer, _ledgeClimbRecoverTimer, _ledgeClimbTimer,
      _attackCoolDownTimer, _attackTimer, _comboTimer };

      _rollTimer.OnStopTimer += () => _rollCoolDownTimer.StartTimer();
      _ledgeClimbTimer.OnStopTimer += () => _ledgeClimbRecoverTimer.StartTimer();
      _attackTimer.OnStopTimer += () => _attackCoolDownTimer.StartTimer();
    }
    private void Updatetimer()
    {
      foreach (Timer timer in timerList)
      {
        timer.Tick(Time.deltaTime);
      }
    }
    #endregion

    #region StateMachine
    void HandleStateMachine()
    {
      stateMachine = new StateMachine();
      var moveState = new PlayerMoveState(this, playerAnimationManager);
      var airState = new PlayerAirState(this, playerAnimationManager);
      var landState = new PlayerLandState(this, playerAnimationManager);
      var attackState = new PlayerAttackState(this, playerAnimationManager);
      var RollState = new PlayerRollState(this, playerAnimationManager);
      var blockState = new PlayerBlockState(this, playerAnimationManager);
      var ledgeClimbState = new PlayerClimbLedgeState(this, playerAnimationManager);

      AddAnyTransition(airState, new FuncPredicate(() => (((IsFalling && !_isGrounded) || (IsJumping && _isGrounded))) && !_rollTimer.IsRunning && !_ledgeClimbTimer.IsRunning));
      AddTransition(airState, landState, new FuncPredicate(() => _landTimer.IsRunning));
      AddTransition(landState, moveState, new FuncPredicate(() => !_landTimer.IsRunning));
      AddTransition(moveState, attackState, new FuncPredicate(() => _attackTimer.IsRunning));
      AddTransition(moveState, RollState, new FuncPredicate(() => _rollTimer.IsRunning));
      AddTransition(moveState, blockState, new FuncPredicate(() => IsBlocking));
      AddAnyTransition(ledgeClimbState, new FuncPredicate(() => _ledgeClimbTimer.IsRunning));
      AddTransition(ledgeClimbState, moveState, new FuncPredicate(() => !_ledgeClimbTimer.IsRunning && !_ledgeClimbRecoverTimer.IsRunning));

      AddAnyTransition(moveState, new FuncPredicate(() => ReturnToMoveState()));
      stateMachine.SetState(moveState);
    }
    private void AddAnyTransition(IState target, IPredicate codition) => stateMachine.AddAnyTransition(target, codition);
    private void AddTransition(IState current, IState target, IPredicate codition) => stateMachine.AddTransition(current, target, codition);
    bool ReturnToMoveState()
    {
      return _isGrounded
      && !IsAttacking
      && !_rollTimer.IsRunning
      && !_landTimer.IsRunning
      && !IsBlocking;
    }
    #endregion

    #region Collision Check
    private void CheckCollision()
    {
      if (!IsJumping)
      {
        if (!_isGrounded)
        {
          _coyoteTimer.StartTimer();
        }
      }
    }
    #endregion

    #region Manage Gravity
    private void ManageGravity()
    {
      if (_rb.linearVelocity.y < 0 && MoveDirection.y < 0)
      {
        IsFastFalling = true;
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

      if (_isGrounded || _coyoteTimer.IsRunning)
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
      IsFalling = _rb.linearVelocity.y < -0.02f && !_isGrounded;
      if (IsFalling)
      {
        IsJumping = false;
        _isFallingLand = true;
      }
      if (IsJumping && _rb.linearVelocity.y < 0)
      {
        IsJumping = false;
        _isJumpFalling = true;
      }
      if ((_isGrounded || _coyoteTimer.IsRunning) && !IsJumping)
      {
        _isJumpCut = false;
        if (!IsJumping)
          _isJumpFalling = false;
      }
      if (CanJump() && _pressedJumpTimer.IsRunning)
      {
        IsJumping = true;
        _isJumpCut = false;
        _isJumpFalling = false;
        Jump();
      }
      if (_isFallingLand && _isGrounded)
      {
        if (IsFastFalling) _landTimer.Reset(_movementStats.fastLandRecoverTime);
        else _landTimer.Reset(_movementStats.landRecoverTime);
        _isFallingLand = false;

        _landTimer.StartTimer();
        IsFalling = false;
        IsFastFalling = false;
      }
    }
    private bool CanJump()
    {
      return (_isGrounded || _coyoteTimer.IsRunning) && !IsJumping;
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
      _pressedJumpTimer.StopTimer();
      _coyoteTimer.StopTimer();

      //We increase the force applied if we are falling
      float force = _movementStats.jumpForce;
      if (_rb.linearVelocity.y < 0)
        force -= _rb.linearVelocity.y;

      _rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }

    #endregion

    #region Roll
    public void RollCheck()
    {
      if (_rollPressTimer.IsRunning)
      {
        _rollTimer.StartTimer();
      }
    }
    public void HandleRoll()
    {
      _rollPressTimer.StopTimer();
      _coyoteTimer.StopTimer();
      var currentGravity = _rb.gravityScale;
      SetGravityScale(0);
      float dir = 0;
      if (MoveDirection.x != 0)
        dir = MoveDirection.normalized.x;
      else
        dir = IsFacingRight ? 1 : -1;

      float force = _movementStats.RollAmount;
      _rb.AddForce(Vector2.right * dir * force, ForceMode2D.Impulse);
    }
    #endregion

    #region Ledge climb
    Vector2 botLedgePos;
    Vector2 topLedgePos;
    public void HandleLedgeClimb()
    {
      if (_isOnLedge)
      {
        if (!_ledgeClimbTimer.IsRunning && !_ledgeClimbRecoverTimer.IsRunning)
        {
          _ledgeClimbTimer.StartTimer();
          if (IsFacingRight)
          {
            botLedgePos = new Vector2(Mathf.Floor(this.transform.position.x * 2) / 2
            , Mathf.Floor((this.transform.position.y + _movementStats.PlayerPosYOffset) * 2) / 2 - _movementStats.PlayerPosYOffset);
            topLedgePos = new Vector2(botLedgePos.x + 1f, botLedgePos.y + _movementStats.PlayerPosYOffset * 2 + 0.5f);
          }
          else
          {
            botLedgePos = new Vector2(Mathf.Ceil(this.transform.position.x * 2) / 2
            , Mathf.Floor((this.transform.position.y + _movementStats.PlayerPosYOffset) * 2) / 2 - _movementStats.PlayerPosYOffset);
            topLedgePos = new Vector2(botLedgePos.x + 1f, botLedgePos.y + _movementStats.PlayerPosYOffset * 2 + 0.5f);
          }
        }
        // float currentGravity = _rb.gravityScale;
        // SetGravityScale(0);
        // this.transform.position = botLedgePos;
        // if (_ledgeClimbTimer.Finished) this.transform.position = topLedgePos;
        // SetGravityScale(currentGravity);
      }
    }

    public void LedgeGrabbing()
    {
      float currentGravity = _rb.gravityScale;
      if (_ledgeClimbTimer.IsRunning)
      {
        SetGravityScale(0);
        this.transform.position = botLedgePos;
      }
      if (_ledgeClimbTimer.Finished) this.transform.position = topLedgePos;
      SetGravityScale(currentGravity);
    }
    #endregion

    #region Attack
    private void AttackCheck()
    {
      //Check if player can attack or not
      if (IsAttacking)
      {
        return;
      }
      if (_isGrounded)
      {
        IsComboAttack = true;
      }
      else
      {
        IsComboAttack = false;
      }
    }
    void HandleAttack()
    {
      if (IsComboAttack)
      {
        if (AttackCount > 0 && !_comboTimer.IsRunning)
        {
          _comboTimer.StopTimer();
        }
        IncreaseAttackCount();
      }
    }
    private void IncreaseAttackCount()
    {
      //Increase the attack count in combo attack
      AttackCount++;
      _comboTimer.StartTimer();
      if (AttackCount > 3 || !_comboTimer.IsRunning)
      {
        AttackCount = 1;
        _comboTimer.StopTimer();
      }
    }
    #endregion

  }
}
