using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Rougelike2D
{
  public class PlayerController : MonoBehaviour
  {
    #region Components
    public static PlayerController Instance { get; private set; }
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private PlayerMovementStats _movementStats;
    [SerializeField] private PlayerCombatStats _combatStats;
    [SerializeField] private List<Attack> _attacks = new List<Attack>();
    private CollisionCheck _collisionCheck;
    private Rigidbody2D _rb;
    PlayerAnimationManager playerAnimationManager;
    StateMachine stateMachine;
    public IHorizontalMovement HorizontalMovement { get; private set; }
    public IJumpMovement JumpMovement { get; private set; }
    public IRollMovement RollMovement { get; private set; }
    public IMeleeAttackAction AttackAction { get; private set; }

    #endregion

    #region Variables
    public Vector2 MoveDirection { get; private set; }
    public Vector2 Velocity { get; private set; }
    public bool IsGrounded => _collisionCheck.IsGrounded;
    private bool _isOnLedge => _collisionCheck.IsOnLedge;
    //Player State
    public bool IsMoving => Mathf.Abs(MoveDirection.x) > 0.01f;
    public bool IsFacingRight { get; private set; }
    public bool IsJumping => JumpMovement.IsJumping;
    public bool IsFalling => JumpMovement.IsFalling;
    public bool IsJumpCut => JumpMovement.IsJumpCut;
    //combat
    public int AttackCount { get; private set; }
    public bool IsAttacking { get; private set; }
    public bool CanAttack { get; private set; }
    public bool IsBlocking { get; private set; }
    public bool IsFastFalling { get; private set; }

    public bool IsComboAttack {get; private set;}
    //Player Timers
    List<Timer> timerList;
    public CountDownTimer CoyoteTimer;
    public CountDownTimer RollPressTimer;
    public CountDownTimer RollTimer;
    public CountDownTimer RollCoolDownTimer;
    public CountDownTimer PressedJumpTimer;
    public CountDownTimer LandTimer;
    public CountDownTimer _ledgeClimbTimer;
    public CountDownTimer _ledgeClimbRecoverTimer;
    public CountDownTimer AttackTimer;
    public CountDownTimer _attackCoolDownTimer;
    public CountDownTimer ComboTimer;

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
      HorizontalMovement = new PlayerHorizontalMovement(transform, _rb, _movementStats);
      JumpMovement = new PlayerJump(this, _rb, _movementStats);
      RollMovement = new PlayerRollMovement(this, _rb, _movementStats);
      AttackAction = new PlayerMeleeAttackAction(this, _combatStats,_attacks);
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
      JumpMovement.JumpCheck();
      RollMovement.RollCheck();
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
      if (jumpPressed && !PressedJumpTimer.IsRunning)
      {
        PressedJumpTimer.StartTimer();
      }
    }
    private void RollInput()
    {
      if (!RollTimer.IsRunning && !RollCoolDownTimer.IsRunning)
      {
        RollPressTimer.StartTimer();
      }
    }
    private void AttackInput(bool attack)
    {
      if (attack && !AttackTimer.IsRunning && !_attackCoolDownTimer.IsRunning)
      {
        Debug.Log("input attack");
        AttackTimer.StartTimer();
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
      CoyoteTimer = new CountDownTimer(_movementStats.coyoteTime);
      RollPressTimer = new CountDownTimer(_movementStats.RollInputBufferTime);
      RollTimer = new CountDownTimer(_movementStats.RollTime);
      RollCoolDownTimer = new CountDownTimer(_movementStats.RollCoolDown);
      PressedJumpTimer = new CountDownTimer(_movementStats.jumpInputBufferTime);
      LandTimer = new CountDownTimer(_movementStats.landRecoverTime);
      _ledgeClimbTimer = new CountDownTimer(_movementStats.LedgeClimbTime);
      _ledgeClimbRecoverTimer = new CountDownTimer(_movementStats.LedgeClimbRecover);
      AttackTimer = new CountDownTimer(_combatStats.AttackTime);
      _attackCoolDownTimer = new CountDownTimer(_combatStats.AttackCoolDown);
      ComboTimer = new CountDownTimer(3f);

      timerList = new List<Timer>()
      {
        CoyoteTimer,
        RollPressTimer,
        RollTimer,
        RollCoolDownTimer,
        PressedJumpTimer,
        LandTimer,
        _ledgeClimbTimer,
        _ledgeClimbRecoverTimer,
        AttackTimer,
        _attackCoolDownTimer,
        ComboTimer
      };

      RollTimer.OnStopTimer += () => RollCoolDownTimer.StartTimer();
      _ledgeClimbTimer.OnStopTimer += () => _ledgeClimbRecoverTimer.StartTimer();
      AttackTimer.OnStopTimer += () => _attackCoolDownTimer.StartTimer();
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

      AddAnyTransition(airState, new FuncPredicate(() => ((IsFalling && !IsGrounded) || (IsJumping && IsGrounded))
      && !RollTimer.IsRunning && !_ledgeClimbTimer.IsRunning && !LandTimer.IsRunning));
      AddTransition(airState, landState, new FuncPredicate(() => LandTimer.IsRunning));
      AddTransition(landState, moveState, new FuncPredicate(() => !LandTimer.IsRunning));
      AddTransition(moveState, attackState, new FuncPredicate(() => AttackTimer.IsRunning));
      AddTransition(moveState, RollState, new FuncPredicate(() => RollTimer.IsRunning));
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
      return IsGrounded
      && !AttackTimer.IsRunning
      && !RollTimer.IsRunning
      && !LandTimer.IsRunning
      && !IsBlocking;
    }
    #endregion

    public void HandleMovement() => HorizontalMovement.HandleHorizontalMovement(MoveDirection.x);
    #region Collision Check
    private void CheckCollision()
    {
      if (!IsJumping)
      {
        if (!IsGrounded)
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
        JumpMovement.IsFastFalling = true;
        //Much higher gravity if holding down
        SetGravityScale(_movementStats.gravityScale * _movementStats.fastFallGravityMult);
        //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, Mathf.Max(_rb.linearVelocity.y, -_movementStats.maxFastFallSpeed));
        return;
      }
      if ((IsJumping || JumpMovement.IsJumpFalling) && Mathf.Abs(_rb.linearVelocity.y) < _movementStats.jumpHangTimeThreshold)
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

    #region Roll
    public void HandleRoll()
    {
      if (MoveDirection.x != 0)
      {
        RollMovement.HandleRoll(MoveDirection.normalized.x);
        RollPressTimer.StopTimer();
      }
      else
      {
        float dir = IsFacingRight ? 1 : -1;
        RollMovement.HandleRoll(dir);
      }
    }
    #endregion

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
  }
}
