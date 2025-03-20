using UnityEngine;
namespace Rougelike2D
{
    [CreateAssetMenu(fileName = "Invincible Strategy", menuName = "Scriptable Objects/Strategy/Damageable Strategy/Invincible Strategy")]
    public class InvincibleStrategy : DamageableStrategy
    {
        public InvincibleStrategy(int MaxHealth) : base(MaxHealth)
        {
            maxHealth = 1;
        }

        public override void Damaged(int dmg)
        {
            //Do nothing, this npc is invincible
        }
    }
}
