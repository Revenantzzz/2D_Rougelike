using System.Collections.Generic;
using UnityEngine;

namespace Rougelike2D
{
    [CreateAssetMenu(fileName = "Range Attack Strategy", menuName = "Scriptable Objects/Strategy/Attack Strategy/Range Attack Strategy")]
    public class RangeAttackStrategy : AttackStrategy
    {
        public Projectile Projectile;
        public Transform AttackPoint;
        List<Projectile> projectilePool;


        private void SpawnProjectile(int num)
        {
            for(int i = 0; i < num; i++)
            {
                Projectile prefabInstance = Object.Instantiate(Projectile, AttackPoint.position, AttackPoint.rotation, AttackPoint);
                projectilePool.Add(prefabInstance);
            }
        }
        public override void Attack()
        {
            projectilePool[0].transform.parent = null;
            
        }
    }
}
