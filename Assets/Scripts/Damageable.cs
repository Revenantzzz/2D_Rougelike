using UnityEngine;
using UnityEngine.Events;

namespace Rougelike2D
{
    public abstract class Damageable: MonoBehaviour
    {
        private int _maxHealth;
        public int MaxHealth => _maxHealth;
        private int _currentHealth;
        public float CurrentHealth => _currentHealth;
        public bool IsDead => CurrentHealth <= 0;

        public UnityEvent OnDeath;
        public UnityEvent OnDamage;

        public void SetMaxHealth(int maxHealth)
        {
            _maxHealth = maxHealth;
            _currentHealth = maxHealth;
        }
        public void TakeDamage(int damage)
        {
            _currentHealth= Mathf.Clamp(_currentHealth - damage, 0, _maxHealth);
            OnDamage?.Invoke();
        }
        public void GetHealed(int healAmount)
        {
            _currentHealth= Mathf.Clamp(_currentHealth + healAmount, 0, _maxHealth);
        }
        public void IncreaseMaxHealth(int amount)
        {
            _maxHealth += amount;
            _currentHealth += amount;
        }
        public void DecreaseMaxHealth(int amount)
        {
            _maxHealth -= amount;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        }
        public void ResetHealth()
        {
            _currentHealth = _maxHealth;
        }
        public void Death() => OnDeath?.Invoke();
    }
}
