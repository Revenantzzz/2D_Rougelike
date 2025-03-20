using UnityEngine;

namespace Rougelike2D
{
    public class NPCSpawn : MonoBehaviour
    {
        [SerializeField] NPCDataSO data;
        NPCFactory nPCFactory;

        public void SpawnNPC()
        {
            nPCFactory = new NPCFactory(data);
            GameObject gameObject = nPCFactory.CreateNPC();
            gameObject.transform.position = this.transform.position;
            gameObject.transform.parent = this.transform;

        }
        void Start()
        {
            SpawnNPC();
        }
    }
}
