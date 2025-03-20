using UnityEngine;

namespace Rougelike2D
{
    public class NPCFactory
    {
        protected NPCDataSO data;

        public NPCFactory(NPCDataSO data)
        {
            this.data = data;
        }
        public GameObject CreateNPC()
        {
            GameObject prefab = Object.Instantiate(data.prefab);
            NPCCharacter characterScript = prefab.GetComponent<NPCCharacter>();
            characterScript.SetSprite(data.sprite);
            characterScript.SetStrategy(data.locomotionStrategy, data.damageableStrategy, data.attackStrategy);
            return prefab;
        }
    }
}
