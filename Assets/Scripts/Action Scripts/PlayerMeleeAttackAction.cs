using System.Collections.Generic;
using NUnit.Framework;

namespace Rougelike2D
{
    public class PlayerMeleeAttackAction : BaseMeleeAttackAciton
    {
        protected PlayerController _playerController;
        protected PlayerCombatStats _playerCombatStats;
        public PlayerMeleeAttackAction(PlayerController controller, PlayerCombatStats combatStats, List<Attack> attacks)
        {
            this._playerController = controller;
            this._playerCombatStats = combatStats;
            foreach (var attack in attacks)
            {
                AttackList.Add(attack);
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
            if (_playerController.IsGrounded)
            {
                IsComboAttack = true;
            }
            else
            {
                IsComboAttack = false;
            }
        }
        public void HandleAttack()
        {
            if (IsComboAttack)
            {
                if (AttackCount > 0 && !_playerController.ComboTimer.IsRunning)
                {
                    _playerController.ComboTimer.StopTimer();
                }
                IncreaseAttackCount();
            }
        }
        private void IncreaseAttackCount()
        {
            //Increase the attack count in combo attack
            AttackCount++;
            _playerController.ComboTimer.StartTimer();
            if (AttackCount > AttackList.Count || !_playerController.ComboTimer.IsRunning)
            {
                AttackCount = 1;
                _playerController.ComboTimer.StopTimer();
            }
        }
        #endregion
    }
}
