using UnityEngine;

namespace Rougelike2D
{
    public class EnemyController : MonoBehaviour
    {
        Rigidbody2D _rb;

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }
    }
}
