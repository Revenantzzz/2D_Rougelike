using UnityEditor.Animations;
using UnityEngine;

namespace Rougelike2D
{
    [CreateAssetMenu(fileName = "EnemyTypeSO", menuName = "Scriptable Objects/EnemyTypeSO")]
    public class EnemyTypeSO : ScriptableObject
    {
        public GameObject EnemyPrefab;
        public AnimatorController Animator;
        public Sprite EnemySprite;
        public float MaxHealth;
        public float MoveSpeed;

        public Transform FirePoint;
        
    }
}
