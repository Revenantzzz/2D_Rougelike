using UnityEngine;

namespace Rougelike2D
{
    public abstract class EnemyFactory
    {
        public abstract GameObject EnemyCreate(EnemyTypeSO enemyType);
    }

    public class MeleeEnemyFactory : EnemyFactory
    {
        public override GameObject EnemyCreate(EnemyTypeSO enemyType)
        {
            EnemyBuilder enemyBuilder = new EnemyBuilder();
            enemyBuilder.SetEnemyPrefab(enemyType.EnemyPrefab);
            enemyBuilder.SetMaxHP(enemyType.MaxHealth);
            enemyBuilder.SetMoveSpeed(enemyType.MoveSpeed);
            return enemyBuilder.Build();
        }
    }
    public class RangedEnemyFactory : EnemyFactory
    {
        public override GameObject EnemyCreate(EnemyTypeSO enemyType)
        {
            EnemyBuilder enemyBuilder = new EnemyBuilder();
            enemyBuilder.SetEnemyPrefab(enemyType.EnemyPrefab);
            enemyBuilder.SetMaxHP(enemyType.MaxHealth);
            enemyBuilder.SetMoveSpeed(enemyType.MoveSpeed);
            enemyBuilder.SetFirePoint(enemyType.FirePoint);
            return enemyBuilder.Build();
        }
    }
}
