using NUnit.Framework;
using UnityEngine;

namespace Rougelike2D
{
    public class CollisionCheck : MonoBehaviour
    {
        private PlayerMovementController _controller;   //Reference to the PlayerMovementController

        [Header("Checks")] 
	    [SerializeField] private Transform _groundCheckPoint;
	    //Size of groundCheck depends on the size of your character generally you want them slightly small than width (for ground) and height (for the wall check)
        [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
        [Space(5)]
        [SerializeField] private Transform _frontWallCheckPoint;
        [SerializeField] private Transform _backWallCheckPoint;
        [SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);

        [Header("Layers & Tags")]
        [SerializeField] private LayerMask _groundLayer;

        public bool IsGrounded {get; private set;}
        public bool IsOnWall {get; private set;}

        private void Awake()
        {
            _controller = GetComponentInParent<PlayerMovementController>();
        }
        private void Update()
        {
            CheckCol();
        }
        private void CheckCol()
        {
            if(!_controller.IsJumping)
            {
                IsGrounded = false;
                IsOnWall = false;

                //Ground Check
                if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer) && !_controller.IsJumping) //checks if set box overlaps with ground
                {
                    IsGrounded = true;
                }	

                //Right Wall Check
                if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && _controller.IsFacingRight)
                    || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !_controller.IsFacingRight)) )
                {
                    IsOnWall = true;
                }

                //Left Wall Check
                else if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !_controller.IsFacingRight)
                    || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && _controller.IsFacingRight)) )
                {
                    IsOnWall = true;
                }
            }        
        }
         private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
            Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);
        }
    }
}
