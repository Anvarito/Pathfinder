using System;
using UnityEngine;

namespace _Workspace.Scripts.TacticalBattle
{
    public class UnitMover : MonoBehaviour
    {
        [SerializeField] private float moveTime = 1f;
        private PathNode _nodeToMove;
        private Vector3 _startPos;
        private bool _runStep = false;
        private float _lerpTime;
        public event Action OnStepEnded;
        public event Action OnMoveStart; 


        public void MoveNext(PathNode target)
        {
            _nodeToMove = target;
            _runStep = true;
            _startPos = transform.position;
            OnMoveStart?.Invoke(); 
        }

        private void Update()
        {
            if (_runStep)
            {
                _lerpTime += Time.deltaTime;
                float alpha = _lerpTime / moveTime;
                Vector3 newPos = Vector3.Lerp(_startPos, _nodeToMove.GridPosition, alpha);
                transform.position = newPos;
                if (alpha >= 1)
                {
                    transform.position = _nodeToMove.GridPosition;
                    _runStep = false;
                    _lerpTime = 0;
                    OnStepEnded?.Invoke();
                }
            }
        }
    }
}