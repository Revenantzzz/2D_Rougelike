using UnityEngine;

namespace Rougelike2D
{
    public class NPCCharacter : MonoBehaviour
    {

        Sprite sprite;

        LocomotionStrategy locomotionStrategy;
        DamageableStrategy damageableStrategy;
        AttackStrategy attackStrategy;

        private Rigidbody _rb;

        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }
        public void SetStrategy(LocomotionStrategy ls, DamageableStrategy ds, AttackStrategy ats)
        {
            locomotionStrategy = ls;
            damageableStrategy = ds;
            attackStrategy = ats;
        }
        public void SetSprite(Sprite sprite)
        {
            GetComponentInChildren<SpriteRenderer>().sprite = sprite;
        }
        public void HorizontalMove() => locomotionStrategy.HorizontalMovement();
        public void Damaged(int dmg) => damageableStrategy.Damaged(dmg);
        public void Attack() => attackStrategy.Attack();
        
    }
}
