using UnityEngine;

namespace Rougelike2D
{
    [CreateAssetMenu(fileName = "AttackStats", menuName = "Scriptable Objects/AttackStats")]
    public class AttackStats : ScriptableObject
    {
        public float DamageScale = 1f;

        public float AnimationLength;

        
    }
}
