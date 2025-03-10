using UnityEngine;

namespace Rougelike2D
{
    public abstract class AttackStrategy: ScriptableObject
    {
        [SerializeField] int damage = 10;
        [SerializeField] float attackRate = 1f;

        public int Damage => damage;
        public float AttackRate => attackRate;

        public virtual void Initialize()
        {
            Debug.Log("Attack Strategy Initialized");
        }
        public abstract void Attack(LayerMask layer);

    }

    [CreateAssetMenu(fileName = "MeleeAtackStrategy", menuName = "Scriptable Objects/AttackStrategy/MeleeAtackStrategy")]
    public class MeleeAtackStrategy : AttackStrategy
    {
        public override void Attack(LayerMask layer)
        {
            
        }
    }
    [CreateAssetMenu(fileName = "RangedAtackStrategy", menuName = "Scriptable Objects/AttackStrategy/RangedAtackStrategy")]
    public class RangedAtackStrategy : AttackStrategy
    {
        [SerializeField] Transform firePoint;
        [SerializeField] GameObject projectilePrefab;
        [SerializeField] float projectileSpeed = 10f;
        [SerializeField] float projectileLifeTime = 2f;
        public override void Attack(LayerMask layer)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            projectile.transform.SetParent(firePoint);
            projectile.layer = layer;
            //Projectile projectileComponent = projectile.GetComponent<Projectile>();
            //projectileComponent.SetSpeed(projectileSpeed);
            Destroy(projectile, projectileLifeTime);
        }
    }
}
