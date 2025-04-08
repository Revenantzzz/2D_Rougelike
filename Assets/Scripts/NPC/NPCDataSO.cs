using UnityEditor;
using UnityEngine;

namespace Rougelike2D
{
    public enum AttackType { Melee, Ranged, NoAttack }
    public enum MoveType {GroundMove, Flying, NoMove}
    public abstract class NPCDataSO : ScriptableObject
    {
        public GameObject prefab;
        public Sprite sprite;
        public bool IsDamageable = true;

        public int Atk;
    }
    [CreateAssetMenu(fileName = "FriendlyNPCData", menuName = "Scriptable Objects/NPCData/FriendlyNPC")]
    public class FriendlyNPCSO : NPCDataSO
    {
        [HideInInspector] public FriendlyNPCCharacter friendlyNPCCharacter;
        public bool CanFollowPlayer;
        void Awake()
        {
            friendlyNPCCharacter = prefab.GetComponent<NPCCharacter>() as FriendlyNPCCharacter;
        }
    }
    public class NeutralNPCSO : NPCDataSO
    {

    }
    [CreateAssetMenu(fileName = "Enemy NPCData", menuName = "Scriptable Objects/NPCData/Enemy NPC")]
    public class EnemyNPCSO : NPCDataSO
    {

    }
}
