using UnityEngine;

namespace Rougelike2D
{
    public class Attacks : MonoBehaviour
    {
        [SerializeField] AttackStats attackStats;

        PolygonCollider2D col;

        void Awake()
        {
            col = GetComponent<PolygonCollider2D>();
        }
    }
}
