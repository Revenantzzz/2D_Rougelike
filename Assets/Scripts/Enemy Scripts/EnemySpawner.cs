using UnityEngine;

namespace Rougelike2D
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] bool _isRangedEnemy = false;
        [SerializeField] EnemyTypeSO _enemyType;

        private EnemyFactory enemyFactory;

        private void Start()
        {
            if (_isRangedEnemy)
            {
                enemyFactory = new RangedEnemyFactory();
            }
            else
            {
                enemyFactory = new MeleeEnemyFactory();
            }

            SpawnEnemy(enemyFactory);
        }
        private void SpawnEnemy(EnemyFactory enemyFactory)
        {
            GameObject enemy = enemyFactory.EnemyCreate(_enemyType);
            enemy.transform.position = transform.position;
        }
    }
}
