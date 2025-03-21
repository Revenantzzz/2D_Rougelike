using System;
using Unity.VisualScripting;
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

            InitializeEvent();
        }
        private void InitializeEvent()
        {
            combatController.OnAttack += PlayerAttack;
            movementController.OnMove += PlayerMove;
            movementController.OnJump += PlayerJump;
            movementController.OnFall += PlayerFall;
            movementController.OnJumpLand += PlayerLand;
        }
        void Update()
        {
            
        }

        private void PlayerMove(Vector2 moveVector, float moveInput)
        {
            if(Mathf.Abs(moveVector.x) > 0.01f && Mathf.Abs(moveVector.y) < 0.01f)
            {
                playerAnimationManager.Play(AnimationString.PlayerRunning, false, false);
            }
            else if(Mathf.Abs(moveInput) < 0.01f)
            {
                playerAnimationManager.Play(AnimationString.PlayerIdle, false, false);
            }
        }
        private void PlayerJump()
        {
            playerAnimationManager.Play(AnimationString.PlayerJump, true, false);
        }
        private void PlayerFall(bool isFalling)
        {
            if(isFalling)
            {
                playerAnimationManager.Play(AnimationString.PlayerFalling, true, false);
            }
        }
        private void PlayerAttack()
        {
            AttackCounter = combatController.AttackCount;
            playerAnimationManager.SetFloatValue("Attack Count", AttackCounter);
            if(combatController.IsComboAttack)
            {
                AttackType = 0f;
            }
            playerAnimationManager.SetFloatValue("Attack Type", AttackType);
            playerAnimationManager.Play(AnimationString.PlayerAttack, true, false);
        }
        private void PlayerLand()
        {
            playerAnimationManager.Play(AnimationString.PlayerLand, true, true);
        }
    }
}
