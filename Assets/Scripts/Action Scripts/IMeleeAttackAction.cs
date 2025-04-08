using System.Collections.Generic;
using UnityEngine;

namespace Rougelike2D
{
    public interface IMeleeAttackAction
    {
        public bool IsAttacking { get; set; }
        public bool IsComboAttack { get; set; }
        public int AttackCount { get; set; }
        public List<Attack> AttackList { get; set; }
    }

    public class BaseMeleeAttackAciton : IMeleeAttackAction
    {
        public bool IsAttacking { get; set; } = false;
        public bool IsComboAttack { get; set; } = true;
        public int AttackCount { get; set; } = 0;
        public List<Attack> AttackList { get; set; } = new List<Attack>();
    }
}
