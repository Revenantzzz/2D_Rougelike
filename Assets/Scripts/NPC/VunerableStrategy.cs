using UnityEngine;
namespace Rougelike2D
{
    [CreateAssetMenu(fileName = "Vunerable Strategy", menuName = "Scriptable Objects/Strategy/Damageable Strategy/Vulnerable Strategy")]
    public class VunerableStrategy : DamageableStrategy
    {
        public VunerableStrategy(int MaxHealth) : base(MaxHealth) {}
        public override void Damaged(int dmg)
        {
            currentHealth = Mathf.Clamp(currentHealth - dmg, 0, maxHealth);
        }
        public int GetCurrentHealth() => currentHealth;
        public int GetMaxHealth() => maxHealth;
        public void IncreaseMaxHealth(int amount)
        {
            maxHealth += amount;
            currentHealth += amount;
        }
        public void DecreaseMaxHealth(int amount)
        {
            maxHealth = Mathf.Clamp(maxHealth - amount, 1, maxHealth);
            currentHealth = Mathf.Clamp(currentHealth,1, maxHealth);
        }
        public int Heal(int amount)
        {
            currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
            return currentHealth;
        }
    }
}
