using System;
using UnityEngine;
using UnityEngine.Events;

namespace Rougelike2D
{
    public enum PlayerState
    {
        Idle,
        Moving,
        Jumping,
        Falling,
        Hit,
        Attacking,
        Blocking,

        Dead
    }
    [RequireComponent(typeof(PlayerMovementController))]
    [RequireComponent(typeof(PlayerCombatController))]
    public class PlayerController : MonoBehaviour
    {
        //Connect all the components of player
        #region Varialbes and Components
        public PlayerController Instance; //Singleton

        [Header("Player Components")]
        [SerializeField] private InputReader _inputReader;
        [SerializeField] private PlayerMovementStats _playerMovementStats;
        [SerializeField] private PlayerCombatStats _playerCombatSatats;
        [SerializeField] private CollisionCheck _collisionCheck;

        //Variables
        public PlayerState CurrentState { get; private set; } 
        public CollisionCheck CollisionCheck { get => _collisionCheck; }  
        public InputReader InputReader { get => _inputReader; } 
        public PlayerMovementStats PlayerMovementStats { get => _playerMovementStats; }
        public PlayerCombatStats PlayerCombatStats { get => _playerCombatSatats; }
        PlayerMovementController movementController;
        PlayerCombatController combatController;
        #endregion

        public UnityAction OnPlayerAttack = delegate{}; //Player Attack Event

        #region In Combat Variables
        public float AttackCounter { get; private set; } //Attack Counter in combo attack
        public float AttackType { get; private set; } //2 types of attack is Combo and Non-Combo
        #endregion

        private void Awake()
        {
            Singelton();
            movementController = GetComponent<PlayerMovementController>();
            combatController = GetComponent<PlayerCombatController>();
        }
        private void Singelton()
        {
            //Singleton
            if(Instance == null)
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

            combatController.OnAttack += PlayerAttack;
        }
        void Update()
        {
            CheckState();
        }

        private void CheckState()
        {
            //Check Player State
            
            if(CurrentState == PlayerState.Dead)
            {
                return;
            }
            if(combatController.IsAttacking)
            {
                CurrentState = PlayerState.Attacking;
                return;
            }
            if(movementController.IsJumping)
            {
                CurrentState = PlayerState.Jumping;
                return;
            }
            if(movementController.IsMoving)
            {
                CurrentState = PlayerState.Moving;
                return;
            }
            CurrentState = PlayerState.Idle;
        }

        private void PlayerAttack()
        {
            AttackCounter = combatController.AttackCount;
            if(combatController.IsComboAttack)
            {
                AttackType = 0f;
            }
            OnPlayerAttack?.Invoke();
        }
    }
}
