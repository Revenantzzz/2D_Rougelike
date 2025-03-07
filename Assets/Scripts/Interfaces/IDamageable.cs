using UnityEngine;
using UnityEngine.Events;

namespace Rougelike2D
{
    public class IDamageable
    {
        private float maxHealth;
        private float currentHealth;
        public bool IsDead => currentHealth <= 0;
        public UnityAction OnHit = delegate { };

        public void TakeDamage(float damage)
        {
            currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        }
        public void GetHealed(float healAmount)
        {
            currentHealth = Mathf.Clamp(currentHealth + healAmount, 0, maxHealth);
        }

    }
}
