using UnityEngine;

namespace Rougelike2D
{
    public class EnemyController : MonoBehaviour
    {
        Rigidbody2D _rb;
        EnemyAnimationManager animationManager;
        private bool _isFacingRight = true;
        public bool _isRanged = false;

        Transform firePoint;
        public void SetFirePoint(Transform firePoint) => this.firePoint = firePoint;

        private float _moveSpeed = 5f;
        public void SetMoveSpeed(float moveSpeed) => _moveSpeed = moveSpeed;

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            animationManager = GetComponentInChildren<EnemyAnimationManager>();
        }

    }
}
