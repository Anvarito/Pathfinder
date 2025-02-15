using System;
using UnityEngine;
using Zenject;

namespace _Workspace.Scripts.PlayerInput
{
    public interface IInputService
    {
        public event Action OnLeftMouseDown;
        public event Action OnLeftMouseUp;
        public event Action OnLeftMousePress;
        public event Action OnRightMouseDown;
        public event Action OnRightMouseUp;
        public event Action OnRightMousePress;
        public event Action OnMiddleMouseDown;
        public event Action OnMiddleMouseUp;
        public event Action OnMiddleMousePress;
        public event Action<float> OnWheelAxis;


        public event Action<float, float> OnMovePress;
        public event Action OnMoveStop;
        public event Action OnDownDirectionDown;
        public event Action OnUpDirectionDown;
        public event Action OnForwardDirectionPress;
        public event Action OnLeftDirectionPress;
        public event Action OnBackDirectionPress;
        public event Action OnRightDirectionPress;
        public event Action OnSprintPress;
        
        public Vector2 MouseAxisMove { get; }
        public Vector2 MiddleMouseOnAxis { get; }
    }
    public class InputService : ITickable, IInputService
    {
        public event Action OnLeftMouseDown;
        public event Action OnLeftMouseUp;
        public event Action OnLeftMousePress;
        public event Action OnRightMouseDown;
        public event Action OnRightMouseUp;
        public event Action OnRightMousePress;
        public event Action OnMiddleMouseDown;
        public event Action OnMiddleMouseUp;
        public event Action OnMiddleMousePress;
        public event Action<float> OnWheelAxis;
        
        public event Action<float, float> OnMovePress;
        public event Action OnMoveStop;
        public event Action OnDownDirectionDown;
        public event Action OnUpDirectionDown;
        public event Action OnForwardDirectionPress;
        public event Action OnLeftDirectionPress;
        public event Action OnBackDirectionPress;
        public event Action OnRightDirectionPress;
        public event Action OnSprintPress;

        public Vector2 MouseAxisMove { get; private set; }
        public Vector2 MiddleMouseOnAxis { get; private set; }

        public void Tick()
        {
            MouseInput();
            KeyboardInput();
        }

        private void KeyboardInput()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                OnDownDirectionDown?.Invoke();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                OnUpDirectionDown?.Invoke();
            }
            
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            if (horizontal != 0 || vertical != 0)
            {
                OnMovePress?.Invoke(horizontal, vertical);
            }
            else
            {
                OnMoveStop?.Invoke();
            }
            
            if (Input.GetKey(KeyCode.W))
            {
                OnForwardDirectionPress?.Invoke();
            }
            if (Input.GetKey(KeyCode.A))
            {
                OnLeftDirectionPress?.Invoke();
            }
            if (Input.GetKey(KeyCode.S))
            {
                OnBackDirectionPress?.Invoke();
            }
            if (Input.GetKey(KeyCode.D))
            {
                OnRightDirectionPress?.Invoke();
            }
            
            if (Input.GetKey(KeyCode.LeftShift))
            {
                OnSprintPress?.Invoke();
            }
        }

        private void MouseInput()
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            MouseAxisMove = new Vector2(mouseX, mouseY);
            
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                OnWheelAxis?.Invoke(scroll);
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                OnLeftMouseDown?.Invoke();
            }
            
            if (Input.GetMouseButton(0))
            {
                OnLeftMousePress?.Invoke();
            }

            if (Input.GetMouseButtonUp(0))
            {
                OnLeftMouseUp?.Invoke();
            }

            if (Input.GetMouseButtonDown(1))
            {
                OnRightMouseDown?.Invoke();
            }

            if (Input.GetMouseButtonUp(1))
            {
                OnRightMouseUp?.Invoke();
            }
            
            if (Input.GetMouseButton(1))
            {
                OnRightMousePress?.Invoke();
            }

            if (Input.GetMouseButtonDown(2))
            {
                OnMiddleMouseDown?.Invoke();
            }
            
            if (Input.GetMouseButton(2))
            {
                OnMiddleMousePress?.Invoke();
            }

            if (Input.GetMouseButtonUp(2))
            {
                OnMiddleMouseUp?.Invoke();
            }
        }
    }
}