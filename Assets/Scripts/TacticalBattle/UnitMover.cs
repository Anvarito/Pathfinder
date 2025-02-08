using System;
using UnityEngine;

namespace TacticalBattle
{
    public class UnitMover : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;

        private bool _isStopMoveComand;
        private PathNode _prevNode;
        private bool _runStep = false;

        private PathNode _nodeToMove;
        public event Action OnStepEnded;

        public void MoveNext(PathNode target)
        {
            _nodeToMove = target;
            _runStep = true;
        }

        private void Update()
        {
            if (_runStep)
            {
                MoveAlongPath();
            }
        }

        private void MoveAlongPath()
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                _nodeToMove.GridPosition,
                moveSpeed * Time.deltaTime
            );

            if (transform.position == _nodeToMove.GridPosition)
            {
                transform.position = _nodeToMove.GridPosition;
                _runStep = false;
                OnStepEnded?.Invoke();
            }
        }
    }
}