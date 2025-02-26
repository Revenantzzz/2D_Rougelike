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
        public UnityAction<bool> OnPlayerAttack = delegate { };
        public UnityAction<bool> OnPlayerInteract = delegate { };
        public UnityAction<bool> OnPlayerCrouch = delegate { };
        public UnityAction<bool> OnPlayerBlock = delegate {};
        public UnityAction OnPlayerDash = delegate{};
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
        public void OnDash(InputAction.CallbackContext context)
        {
            if(context.started) OnPlayerDash?.Invoke();
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if(context.started)
            {
                OnPlayerAttack?.Invoke(true);
            }
            else if(context.canceled)
            {
                OnPlayerAttack?.Invoke(false);
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            if(context.started)
            {
                OnPlayerCrouch?.Invoke(true);
            }
            else if(context.canceled)
            {
                OnPlayerCrouch?.Invoke(false);
            }
        }
        public void OnBlock(InputAction.CallbackContext context)
        {
            if(context.started)
            {
                OnPlayerCrouch?.Invoke(true);
            }
            else if(context.canceled)
            {
                OnPlayerCrouch?.Invoke(false);
            }
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
    
