using UnityEngine;
namespace Rougelike2D
{
    public abstract class DamageableStrategy : ScriptableObject        
    {
        protected int maxHealth;
        protected int currentHealth;

        public DamageableStrategy(int MaxHealth)
        {
            this.maxHealth = Mathf.Max(MaxHealth, 1);
            this.currentHealth = maxHealth;
        }
        public abstract void Damaged(int dmg);
    }
}
