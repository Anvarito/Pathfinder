using System;
using UnityEngine;
using Zenject;

namespace _Workspace.Scripts.PlayerInput
{
    public interface IInputService
    {
        public event Action OnLeftMouseDown;
        public event Action OnLeftMouseUp;
        public event Action OnRightMouseDown;
        public event Action OnRightMouseUp;
        public event Action OnMiddleMouseDown;
        public event Action OnMiddleMouseUp;
    }
    public class InputService : ITickable, IInputService
    {
        public event Action OnLeftMouseDown;
        public event Action OnLeftMouseUp;
        public event Action OnRightMouseDown;
        public event Action OnRightMouseUp;
        public event Action OnMiddleMouseDown;
        public event Action OnMiddleMouseUp;
        
        public void Tick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnLeftMouseDown?.Invoke();
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
            
            if (Input.GetMouseButtonDown(2))
            {
                OnMiddleMouseDown?.Invoke();
            }
            if (Input.GetMouseButtonUp(2))
            {
                OnMiddleMouseUp?.Invoke();
            }
        }
    }
}