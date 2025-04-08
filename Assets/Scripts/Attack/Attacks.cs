using UnityEngine;

namespace Rougelike2D
{
    public class Attack : MonoBehaviour
    {
        private int damage = 0;

        public Attack()
        {
        }

        public void SetDmg(int dmg) => damage = dmg;
        public int GetDmg() => damage;
        
    }
}
