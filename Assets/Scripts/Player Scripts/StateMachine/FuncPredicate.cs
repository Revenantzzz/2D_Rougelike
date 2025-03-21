using System;

namespace Rougelike2D
{
    public class FuncPredicate : IPredicate
    {
        Func<bool> func;
        public FuncPredicate(Func<bool> func)
        {
            this.func = func;
        }
        public bool Evaluate()
        {
            return func.Invoke();
        }
    }
}
