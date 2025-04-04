namespace Rougelike2D
{
    public class Transition : ITransition
    {
        public IState TargetState {get;}

        public IPredicate Condition {get;}

        public Transition(IState state, IPredicate condition)
        {
            this.TargetState = state;
            this.Condition = condition;
        }
    }
}
