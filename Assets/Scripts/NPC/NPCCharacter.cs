using UnityEngine;

namespace Rougelike2D
{
    public class NPCCharacter : MonoBehaviour
    {

        Sprite sprite;

        private Rigidbody _rb;

        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }
        public void SetSprite(Sprite sprite)
        {
            GetComponentInChildren<SpriteRenderer>().sprite = sprite;
        }
        
    }
}
