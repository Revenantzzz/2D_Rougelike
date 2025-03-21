using Unity.VisualScripting;
using UnityEngine;

namespace Rougelike2D
{
    public interface IPredicate
    {
        public bool Evaluate();
    }
}
