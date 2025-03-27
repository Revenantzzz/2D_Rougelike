using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace Rougelike2D
{
    public class CollisionCheck : MonoBehaviour
    {
        private PlayerMovementController _controller;   //Reference to the PlayerMovementController

        [Header("Checks")] 
	    [SerializeField] private Transform _groundCheckPoint;
	    //Size of groundCheck depends on the size of your character generally you want them slightly small than width (for ground) and height (for the wall check)
        [SerializeField] private UnityEngine.Vector2 _groundCheckSize = new UnityEngine.Vector2(0.49f, 0.03f);
        [Space(5)]
        [SerializeField] private Transform _rightWallCheckPoint;
        [SerializeField] private Transform _leftWallCheckPoint;
        [SerializeField] private Transform _cliffCheckPoint;
        [SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);

        [Header("Layers & Tags")]
        [SerializeField] private LayerMask _groundLayer;

        public bool IsGrounded {get; private set;}
        public bool IsOnWall {get; private set;}
        public bool IsOnCliff {get; private set;}

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
                if (Physics2D.OverlapBox(_rightWallCheckPoint.position, _wallCheckSize, 0, _groundLayer))
                {
                    IsOnWall = true;
                    // if(!Physics2D.Raycast(_cliffCheckPoint.position, UnityEngine.Vector2.right, 2f, _groundLayer))
                    // {
                    //     IsOnCliff = true;
                    //     Debug.Log("cliff");
                    // }
                }

                //Left Wall Check
                else if (Physics2D.OverlapBox(_leftWallCheckPoint.position, _wallCheckSize, 0, _groundLayer))
                {
                    // IsOnWall = true;
                    // if(!Physics2D.Raycast(_cliffCheckPoint.position, Vector2.left, 2f, _groundLayer))
                    // {
                    //     IsOnCliff = true;
                    // }
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
            //Gizmos.DrawLine(_cliffCheckPoint.position, new Vector2(1, 0));
            //Gizmos.DrawLine(_cliffCheckPoint.position, Vector2.left);
        }
    }
}
