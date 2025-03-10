using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;

namespace Rougelike2D
{
    public class PlayerCombatController : Damageable
    {
        [SerializeField] private PlayerCombatStats _playerCombatStats;
        PlayerController _playerController;
        private InputReader _inputReader =>_playerController.InputReader;
        private CollisionCheck _collisionCheck => _playerController.CollisionCheck;

        public int AttackCount{get; private set;}
        public bool IsAttacking{get; private set;}
        public bool CanAttack {get; private set;}

        private float _comboTimer = 0f;

        public UnityAction OnAttack = delegate{};

        public bool IsComboAttack;  
          
        private void Awake() 
        {
            _playerController = GetComponent<PlayerController>();
            SetMaxHealth(_playerCombatStats.MaxHealth);
        }
        private void Start() 
        {
            _inputReader.Enable();
            _inputReader.OnPlayerAttack += AttackInput;

            CanAttack = true;
            AttackCount = 0;
        }
        void Update()
        {
            AttackCheck();

            if(_comboTimer >= 0) _comboTimer -= Time.deltaTime;
        }

        private void AttackInput(bool attack)
        {
            if(attack && CanAttack)
            {
                StartCoroutine(Attack());
            }
        }

        #region Attack
        private void AttackCheck()
        {
            //Check if player can attack or not
            if(IsAttacking)
            {
                return;
            }
            if(_collisionCheck.IsGrounded)
            {
                IsComboAttack = true;
            }
            else
            {
                IsComboAttack = false;
            }
        }
        IEnumerator Attack()
        {
            //Excute Attack
            IsAttacking = true;
            CanAttack = false;
            IncreaseAttackCount();
            OnAttack?.Invoke();

            yield return new WaitForSeconds(1f); //Wait for the attack animation to finish
            
            //After done the attack, then set the IsAttacking to false
            IsAttacking = false;
            //If player performed a combo attack then set _comboTimer then play can perform next attack in combo
            //if not, start the attack cooldown
            if(IsComboAttack && _comboTimer > 0)
            {
                _comboTimer = 1.25f;
                CanAttack = true;
            }
            else
            {       
                StartCoroutine(AttackCooldown());
            }
        }
        IEnumerator AttackCooldown()
        {
            //if player stop attacking for a while
            yield return new WaitForSeconds(.25f);
            CanAttack = true;
            AttackCount = 0;
            _comboTimer = 0f;
        } 
        private void IncreaseAttackCount()
        {
            //Increase the attack count in combo attack
            AttackCount++;
            _comboTimer = 1.25f;
            if(AttackCount > 3)
            {
                AttackCount = 1;
                _comboTimer = 0f;
            }
        }
        #endregion

        
    }
}
