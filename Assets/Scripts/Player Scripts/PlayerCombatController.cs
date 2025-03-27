using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;

namespace Rougelike2D
{
    public class PlayerCombatController : MonoBehaviour
    {
        private PlayerCombatStats _stats => _playerController.PlayerCombatStats;
        PlayerController _playerController;
        private InputReader _inputReader => _playerController.InputReader;
        private CollisionCheck _collisionCheck => _playerController.CollisionCheck;

        public int AttackCount { get; private set; }
        public bool IsAttacking { get; private set; }
        public bool CanAttack { get; private set; }

        public UnityAction OnAttack = delegate { };

        public bool IsComboAttack;
        List<Timer> timerList;
        public CountDownTimer AttackTimer;
        CountDownTimer attackCoolDownTimer;
        CountDownTimer comboTimer;

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            SetupTimer();
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

            UpdateTimer();
        }
        private void SetupTimer()
        {
            AttackTimer = new CountDownTimer(_stats.AttackTime);
            attackCoolDownTimer = new CountDownTimer(_stats.AttackCoolDown);
            comboTimer = new CountDownTimer(3f);

            AttackTimer.OnStartTimer += () => HandleAttack();
            AttackTimer.OnStopTimer += () => attackCoolDownTimer.StartTimer();
            comboTimer.OnStopTimer += () => {AttackCount = 0;};

            timerList = new(3) { AttackTimer, attackCoolDownTimer, comboTimer };
        }
        private void UpdateTimer()
        {
            foreach (Timer timer in timerList)
            {
                timer.Tick(Time.deltaTime);
            }
        }
        private void AttackInput(bool attack)
        {
            if (attack && CanAttack && !AttackTimer.IsRunning && !attackCoolDownTimer.IsRunning)
            {
                AttackTimer.StartTimer();
            }
        }

        #region Attack
        private void AttackCheck()
        {
            //Check if player can attack or not
            if (IsAttacking)
            {
                return;
            }
            if (_collisionCheck.IsGrounded)
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
                if (AttackCount > 0 && !comboTimer.IsRunning)
                {
                    comboTimer.StopTimer();
                }
                IncreaseAttackCount();
            }
            OnAttack.Invoke();
        }
        private void IncreaseAttackCount()
        {
            //Increase the attack count in combo attack
            AttackCount++;
            comboTimer.StartTimer();
            if (AttackCount > 3 || !comboTimer.IsRunning)
            {
                AttackCount = 1;
                comboTimer.StopTimer();
            }
        }
        #endregion        
    }
}
