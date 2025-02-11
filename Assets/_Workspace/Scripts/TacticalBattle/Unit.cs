using System;
using System.Collections;
using System.Collections.Generic;
using _Workspace.Scripts.Data.Scripts;
using Extra;
using UnityEngine;

namespace _Workspace.Scripts.TacticalBattle
{
    public class Unit : MonoBehaviour
    {
        [SerializeField] private UnitMover unitMover;
        [SerializeField] private UnitRotator UnitRotator;
        [SerializeField] private float _staepForSeconds;
        public UnitHighlighter UnitHighlighter;
        public PathNode CurrentPathNode { get; private set; }
        public UnitStats Stats => _unitStats;
        public bool IsOnAction { get; private set; }
        
        private UnitStats _unitStats;
        private List<PathNode> _path;
        private MathOperations _mathOperations;
        private PathNode _targetNode;

        private int _nodeIndex = 1;
        private bool _isNeedMove;

        public event Action OnAllActionStop;


        public void Init(PathNode initialNode, Vector3 direction,
            MathOperations mathOperations, UnitStats unitStats)
        {
            CurrentPathNode = initialNode;
            _unitStats = unitStats;
            transform.position = initialNode.GridPosition;
            transform.rotation = Quaternion.Euler(direction);

            _mathOperations = mathOperations;
            UnitHighlighter = GetComponent<UnitHighlighter>();
            
            unitMover.OnStepEnded += MoveStepEnded;
            UnitRotator.OnStepEnded += RotationStepEnd;
        }

        private void MoveStepEnded()
        {
            _nodeIndex++;

            if (_nodeIndex < _path.Count && _isNeedMove)
            {
                StartCoroutine(AwaitAfterMove());
            }
            else
            {
                MoveComplete();
            }
        }

        private void MoveComplete()
        {
            IsOnAction = false;
            _isNeedMove = false;
            _path = null;
            _nodeIndex = 1;
            OnAllActionStop?.Invoke();
        }

        private void RotationStepEnd()
        {
            DecreaseRotationCost();

            if (IsCountAndCostEnough())
            {
                StartCoroutine(AwaitAfterRotation());
            }
            else
            {
                if (_isNeedMove)
                    StartCoroutine(AwaitAfterMove());
                else
                {
                    IsOnAction = false;
                }
            }
        }

        private bool IsCountAndCostEnough()
        {
            var count = GetRotationCount();
            bool costEnough = _unitStats.CurrentActionPoints.value >= Constants.ROTATION_COST;

            return count > 0 && costEnough;
        }

        private IEnumerator AwaitAfterRotation()
        {
            yield return new WaitForSeconds(_staepForSeconds);
            UnitRotator.RotateToNext(_targetNode);
        }

        private IEnumerator AwaitAfterMove()
        {
            yield return new WaitForSeconds(_staepForSeconds);
            RunPath();
        }

        private void RunPath()
        {
            _targetNode = _path[_nodeIndex];

            if (GetRotationCount() == 0)
            {
                CurrentPathNode.UnitCurrent = null;
                _targetNode.UnitCurrent = this;
                DecreaseMoveCost();
                CurrentPathNode = _targetNode;
                unitMover.MoveNext(_targetNode);
            }
            else
            {
                UnitRotator.RotateToNext(_targetNode);
            }
        }

        private int GetRotationCount()
        {
            int count = _mathOperations.CalculateRotationCount(transform.eulerAngles.y, CurrentPathNode,
                _targetNode);
            return count;
        }

        public void ApproveMove(List<PathNode> pathNodes)
        {
            _path = pathNodes;
            _isNeedMove = true;
            IsOnAction = true;
            RunPath();
        }

        public void StopMoving()
        {
            _isNeedMove = false;
        }

        public void LookAtNode(PathNode lookAtNode)
        {
            _targetNode = lookAtNode;
            if (!IsCountAndCostEnough())
                return;

            IsOnAction = true;
            UnitRotator.RotateToNext(lookAtNode);
        }

        private void DecreaseRotationCost()
        {
            _unitStats.DecreaseActionPoints(Constants.ROTATION_COST);
        }

        private void DecreaseMoveCost()
        {
            var cost = _mathOperations.CalculateStepCost(CurrentPathNode, _targetNode, Constants.STEP_COST);
            _unitStats.DecreaseActionPoints(cost);
        }

        public class UnitTransformSaveData
        {
            public Vector3Int NodePosition;
            public Vector3 RotationDir;
        }

        public UnitTransformSaveData GetSaveData()
        {
            UnitTransformSaveData transformSaveData = new UnitTransformSaveData()
            {
                NodePosition = CurrentPathNode.GridPosition,
                RotationDir = transform.eulerAngles
            };

            return transformSaveData;
        }
    }
}