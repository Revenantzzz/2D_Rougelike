using System;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Events;

namespace Rougelike2D
{
    [RequireComponent(typeof(PlayerMovementController))]
    [RequireComponent(typeof(PlayerCombatController))]
    public class PlayerController : MonoBehaviour
    {
        //Connect all the components of player
        #region Varialbes and Components
        public static PlayerController Instance; //Singleton

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            Instance = null;
        }

        [Header("Player Components")]
        [SerializeField] private InputReader _inputReader;
        [SerializeField] private PlayerMovementStats _playerMovementStats;
        [SerializeField] private PlayerCombatStats _playerCombatSatats;
        [SerializeField] private CollisionCheck _collisionCheck;

        //Variables
        Animator animator;
        public Rigidbody2D RB { get; private set; }
        public CollisionCheck CollisionCheck { get => _collisionCheck; }
        public InputReader InputReader { get => _inputReader; }
        public PlayerMovementStats PlayerMovementStats { get => _playerMovementStats; }
        public PlayerCombatStats PlayerCombatStats { get => _playerCombatSatats; }
        PlayerMovementController movementController;
        PlayerCombatController combatController;
        PlayerAnimationManager playerAnimationManager;

        #endregion

        public UnityAction OnPlayerAttack = delegate { }; //Player Attack Event


        #region Variables
        public bool IsMovingInput => movementController.IsMoving;
        public bool IsAttacking => combatController.AttackTimer.IsRunning;
        public bool IsJumping => movementController.IsJumping;
        public bool IsFalling => movementController.IsFalling;
        public bool IsLanding => movementController.LandTimer.IsRunning;
        public bool IsRolling => movementController.RollTimer.IsRunning;
        public float AttackCounter { get; private set; } //Attack Counter in combo attack
        public float AttackType { get; private set; } //2 types of attack is Combo and Non-Combo
        #endregion

        StateMachine stateMachine;

        private void Awake()
        {
            Singelton();
            animator = GetComponentInChildren<Animator>();
            movementController = GetComponent<PlayerMovementController>();
            combatController = GetComponent<PlayerCombatController>();
            playerAnimationManager = GetComponentInChildren<PlayerAnimationManager>();
            RB = GetComponent<Rigidbody2D>();
            HandleStateMachine();
        }
        private void Singelton()
        {
            //Singleton
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }
        private void Start()
        {
            _inputReader.Enable();
        }
        void Update()
        {
            stateMachine.Update();
        }
        void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }
        void HandleStateMachine()
        {
            stateMachine = new StateMachine();
            var moveState = new PlayerMoveState(this, playerAnimationManager);
            var airState = new PlayerAirState(this, playerAnimationManager);
            var landState = new PlayerLandState(this, playerAnimationManager);
            var attackState = new PlayerAttackState(this, playerAnimationManager);
            var RollState = new PlayerRollState(this, playerAnimationManager);

            AddAnyTransition(airState, new FuncPredicate(() => ((IsFalling && !CollisionCheck.IsGrounded) || (IsJumping && CollisionCheck.IsGrounded)) && !IsRolling));
            AddTransition(airState, landState, new FuncPredicate(() => movementController.LandTimer.IsRunning));
            AddTransition(landState, moveState, new FuncPredicate(() => !movementController.LandTimer.IsRunning));
            AddTransition(moveState, attackState, new FuncPredicate(() => combatController.AttackTimer.IsRunning));
            AddTransition(moveState, RollState, new FuncPredicate(() => movementController.RollTimer.IsRunning));

            AddAnyTransition(moveState, new FuncPredicate(() => ReturnToMoveState()));
            stateMachine.SetState(moveState);
        }
        public void AddAnyTransition(IState target, IPredicate codition) => stateMachine.AddAnyTransition(target, codition);
        public void AddTransition(IState current, IState target, IPredicate codition) => stateMachine.AddTransition(current, target, codition);
        bool ReturnToMoveState()
        {
            return CollisionCheck.IsGrounded 
            && !IsAttacking 
            && !IsRolling
            && !IsLanding;
        }
        public void HandleMovement() => movementController.Run();
        public void HandleJump() => movementController.JumpCheck();
        public void StopMovement() => movementController.StopMovement();
        public void HandleRoll () => movementController.Roll();
    }
}
