using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static InputSystem_Actions;

namespace Rougelike2D
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Scriptable Objects/InputReader")]
    public class InputReader : ScriptableObject, IPlayerActions
    {
        private InputSystem_Actions _inputActions;

        public UnityAction<bool> OnPlayerJump = delegate { };
        public Vector2 Move => _inputActions.Player.Move.ReadValue<Vector2>();

        public void Enable()
        {
            if (_inputActions == null)
            {
                _inputActions = new InputSystem_Actions();
                _inputActions.Player.SetCallbacks(this);
            }
            _inputActions.Enable();
        }

        public void Disable()
        {
            _inputActions.Disable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            
        }
        public void OnJump(InputAction.CallbackContext context)
        {
            if(context.started)
            {
                OnPlayerJump?.Invoke(true);
            }
            else if(context.canceled)
            {
                OnPlayerJump?.Invoke(false);
            }
        }
        public void OnLook(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnAttack(InputAction.CallbackContext context)
        {

        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnPrevious(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnNext(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
    
