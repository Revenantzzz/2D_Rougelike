using UnityEngine;
namespace Rougelike2D
{
    public abstract class AttackStrategy : ScriptableObject
    {
        protected int damage;

        public abstract void Attack();
    }
}
