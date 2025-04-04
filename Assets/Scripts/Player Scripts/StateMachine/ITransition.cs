namespace Rougelike2D
{
    public interface ITransition
    {
        IState TargetState {get;}
        IPredicate Condition {get;}
    }
}
