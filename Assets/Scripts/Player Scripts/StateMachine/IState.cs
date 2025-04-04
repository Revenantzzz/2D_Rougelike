
namespace Rougelike2D
{
    public interface IState
    {
        public void StateUpdate();
        public void StateFixedUpdate();
        public void EnterState();
        public void ExitState();
        
    }
}
