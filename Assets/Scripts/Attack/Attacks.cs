using UnityEngine;

namespace Rougelike2D
{
    public class Attacks : MonoBehaviour
    {
        private int damage = 0;

        public void SetDmg(int dmg) => damage = dmg;
        public int GetDmg() => damage;
        
    }
}
