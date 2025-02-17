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
        PlayerAnimationManager playerAnimationManager;
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
            playerAnimationManager = GetComponentInChildren<PlayerAnimationManager>();
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
            movementController.OnJumpLand += PlayerLand;
        }
        void Update()
        {
            CheckState();
        }
        #region Check Player State and Animation
        private void CheckState()
        {
            //Check Player State and Change Animation depending on the state
            switch(playerAnimationManager.CurrentAnimation)
            {
                case "Player_Attack":
                    return;
                case "Player_Jump":
                    return;
                case "Player_Fall":
                    return;
                case "Player_Land":
                    return;
                case "Player_ToRun":
                    return; 
                case "Player_BreakRun":
                    return;
            }
            if(movementController.IsJumping)
            {
                playerAnimationManager.ChangeAnimation(AnimationString.PlayerJump);
                return;
            }
            if(movementController.IsFalling)
            {
                playerAnimationManager.ChangeAnimation(AnimationString.PlayerFall);
                return;
            }
            if(movementController.IsMoving)
            {
                if(playerAnimationManager.CurrentAnimation != AnimationString.PlayerRunning)
                    playerAnimationManager.ChangeAnimation(AnimationString.PlayerToRun);
                return;
            }
            playerAnimationManager.ChangeAnimation("Player_Idle");
        }
        #endregion

        private void PlayerAttack()
        {
            AttackCounter = combatController.AttackCount;
            if(combatController.IsComboAttack)
            {
                AttackType = 0f;
            }
            playerAnimationManager.ChangeAnimation("Player_Attack", 0.1f, 0);
        }
        private void PlayerLand()
        {
            playerAnimationManager.ChangeAnimation("Player_Land", 0.1f, 0);
        }
    }
}
