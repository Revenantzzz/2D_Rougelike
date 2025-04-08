using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace Rougelike2D
{
    public class CollisionCheck : MonoBehaviour
    {
        private PlayerController _controller;   //Reference to the PlayerMovementController

        [Header("Checks")]
        [SerializeField] private Transform _groundCheckPoint;
        //Size of groundCheck depends on the size of your character generally you want them slightly small than width (for ground) and height (for the wall check)
        [SerializeField] private UnityEngine.Vector2 _groundCheckSize = new UnityEngine.Vector2(0.49f, 0.03f);
        [Space(5)]
        [SerializeField] private Transform _rightWallCheckPoint;
        [SerializeField] private Transform _leftWallCheckPoint;
        [SerializeField] private Transform _ledgeCheckPoint;
        [SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);
        [SerializeField] private float _ledgeUnderCheckHeightOffset = .15f;

        [Header("Layers & Tags")]
        [SerializeField] private LayerMask _groundLayer;

        public bool IsGrounded { get; private set; }
        public bool IsOnWall { get; private set; }
        public bool IsOnLedge { get; private set; }

        private void Awake()
        {
            _controller = GetComponentInParent<PlayerController>();
        }
        private void Update()
        {
            CheckCol();
        }
        private void CheckCol()
        {
            if (!_controller.IsJumping)
            {
                IsGrounded = false;
                //Ground Check
                if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer) && !_controller.IsJumping && !_controller.IsFalling) //checks if set box overlaps with ground
                {
                    IsGrounded = true;
                }
            }
            IsOnWall = false;
            IsOnLedge = false;
            //Right Wall Check
            if (Physics2D.OverlapBox(_rightWallCheckPoint.position, _wallCheckSize, 0, _groundLayer))
            {
                IsOnWall = true;
                if (!Physics2D.Raycast(_ledgeCheckPoint.position, Vector2.right, 5f, _groundLayer) &&
               Physics2D.Raycast(_ledgeCheckPoint.position - new UnityEngine.Vector3(0, _ledgeUnderCheckHeightOffset, 0), Vector2.right, 3f, _groundLayer))
                {
                    IsOnLedge = true;
                }
            }
            //Left Wall Check
            if (Physics2D.OverlapBox(_leftWallCheckPoint.position, _wallCheckSize, 0, _groundLayer))
            {
                IsOnWall = true;
                if (!Physics2D.Raycast(_ledgeCheckPoint.position, Vector2.left, 5f, _groundLayer) &&
                Physics2D.Raycast(_ledgeCheckPoint.position - new UnityEngine.Vector3(0, _ledgeUnderCheckHeightOffset, 0), Vector2.left, 3f, _groundLayer))
                {
                    IsOnLedge = true;
                }
            }
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(_rightWallCheckPoint.position, _wallCheckSize);
            Gizmos.DrawWireCube(_leftWallCheckPoint.position, _wallCheckSize);
            Gizmos.DrawRay(_ledgeCheckPoint.position, Vector2.right);
        }
    }
}
