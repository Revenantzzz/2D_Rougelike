using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;

namespace Rougelike2D
{
    public class PlayerCombatController : MonoBehaviour
    {
        PlayerController _playerController;
        private InputReader _inputReader =>_playerController.InputReader;
        private CollisionCheck _collisionCheck => _playerController.CollisionCheck;

        public int AttackCount{get; private set;}
        public bool IsAttacking{get; private set;}
        public bool CanAttack {get; private set;}

        public UnityAction OnAttack = delegate{};

        public bool IsComboAttack;  
          
        private void Awake() 
        {
            _playerController = GetComponent<PlayerController>();
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
        }

        private void AttackInput(bool attack)
        {
            if(attack && CanAttack)
            {
                StartCoroutine(Attack());
            }
        }
        private void AttackCheck()
        {
            //Check if player can attack or not
            if(IsAttacking)
            {
                return;
            }
            if(!_collisionCheck.IsGrounded)
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

            yield return new WaitForSeconds(.5f); //Wait for the attack animation to finish
            
            //After done the attack, then set the IsAttacking to false
            IsAttacking = false;
            if(IsComboAttack)
            {
                CanAttack = true;
            }
            else
            {       
                StartCoroutine(AttackCooldown());
            }
        }
        IEnumerator AttackCooldown()
        {
            //if player stop attacking, then wait for half of a second to attack again
            yield return new WaitForSeconds(.5f);
            CanAttack = true;
            AttackCount = 0;
        } 
        private void IncreaseAttackCount()
        {
            //Increase the attack count in combo attack
            AttackCount++;
            if(AttackCount > 3)
            {
                AttackCount = 1;
            }
        }
    }
}
