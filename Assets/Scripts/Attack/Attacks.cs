using UnityEngine;

namespace Rougelike2D
{
    public class Attacks : MonoBehaviour
    {
        [SerializeField] AttackStrategy attackStrategy;

        void Start()
        {
            attackStrategy.Initialize();
        }
        public void SetStrategy(AttackStrategy strategy)
        {
            attackStrategy = strategy;
            attackStrategy.Initialize();
        }
    }
}
