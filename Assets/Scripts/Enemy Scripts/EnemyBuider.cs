using UnityEngine;

namespace Rougelike2D
{
    public class EnemyBuilder
    {
        GameObject enemyPrefab;
        GameObject weaponPrefab;
        Transform FirePoint;
        float maxHealth;
        float moveSpeed;

        public EnemyBuilder SetEnemyPrefab(GameObject enemyPrefab)
        {
            this.enemyPrefab = enemyPrefab;
            return this;
        }
        public EnemyBuilder SetWeaponPrefab(GameObject weaponPrefab)
        {
            this.weaponPrefab = weaponPrefab;
            return this;
        }
        public EnemyBuilder SetMaxHP(float maxHealth)
        {
            this.maxHealth = maxHealth;
            return this;
        }
        public EnemyBuilder SetMoveSpeed(float moveSpeed)
        {
            this.moveSpeed = moveSpeed;
            return this;
        }
        public EnemyBuilder SetFirePoint(Transform FirePoint)
        {
            this.FirePoint = FirePoint;
            return this;
        }
        public GameObject Build()
        {
            GameObject enemy = GameObject.Instantiate(enemyPrefab);
            //enemy.GetComponent<EnemyController>().SetMaxHP(maxHealth);
            //enemy.GetComponent<EnemyController>().SetMoveSpeed(moveSpeed);
            //enemy.GetComponent<EnemyController>().SetFirePoint(FirePoint);
            return enemy;
        }
    }
}
