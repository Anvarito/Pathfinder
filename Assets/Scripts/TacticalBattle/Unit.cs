using System;
using System.Collections;
using System.Collections.Generic;
using Extra;
using UnityEngine;

namespace TacticalBattle
{
    [Serializable]
    public class UnitMoveCosts
    {
        public float StepCost;
        public float RotationCost;
    }

    [Serializable]
    public class UnitActionParams
    {
        public UnitMoveCosts moveParams;
        public float ActionPoints;
    }

    public class Unit : MonoBehaviour
    {
        [SerializeField] private GameObject _marker;
        [SerializeField] private UnitActionParams _actionParams;
        private PathFinder _pathFinder;
        public PathDrawer _PathDrawer;
        public UnitMover unitMover;
        public UnitRotator UnitRotator;
        private List<PathNode> _path;
        private int _nodeIndex = 1;
        public float _staepForSeconds;
        private MathOperations _mathOperations;
        private bool _isNeedMove;
        private PathNode _targetNode;
        private PathNode _currentNode;
        public bool IsOnAction { get; private set; }

        public Vector3Int PositionInt =>
            new Vector3Int(
                Mathf.RoundToInt(transform.position.x),
                Mathf.RoundToInt(transform.position.y),
                Mathf.RoundToInt(transform.position.z)
            );

        public void Init(PathFinder pathFinder, PathNode initialNode)
        {
            _pathFinder = pathFinder;
            _currentNode = initialNode;
            _mathOperations = new MathOperations();
            unitMover.OnStepEnded += MoveStepEnded;
            UnitRotator.OnStepEnded += RotationStepEnd;
            
            Selected(false);
        }

        private void MoveStepEnded()
        {
            _nodeIndex++;

            if (_nodeIndex < _path.Count)
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
        }

        private void RotationStepEnd()
        {
            DecreaseRotationCost();

            if (IsCountOrCostEnough())
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

        private bool IsCountOrCostEnough()
        {
            var count = GetRotationCount(_targetNode);
            bool costEnough = _actionParams.ActionPoints >= _actionParams.moveParams.RotationCost;
            return count > 0 && costEnough;
        }

        private IEnumerator AwaitAfterRotation()
        {
            yield return new WaitForSeconds(0.1f);
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

            if (GetRotationCount(_targetNode) == 0)
            {
                _currentNode.UnitCurrent = null;
                _targetNode.UnitCurrent = this;
                DecreaseMoveCost();
                _currentNode = _targetNode;
                unitMover.MoveNext(_targetNode);
            }
            else
            {
                UnitRotator.RotateToNext(_targetNode);
            }

            HidePath();
        }

        private int GetRotationCount(PathNode pathNode)
        {
            int count = _mathOperations.CalculateRotationCount(transform.eulerAngles.y, PositionInt,
                pathNode.GridPosition);
            return count;
        }


        public void SearchPath(PathNode node)
        {
            if (node.IsOcupied || _actionParams.ActionPoints < _actionParams.moveParams.StepCost)
            {
                HidePath();
                return;
            }

            _path = _pathFinder.FindPath(
                _currentNode,
                node,
                _actionParams,
                transform.rotation.eulerAngles.y
            );


            if (_path != null)
                _PathDrawer.UpdatePathVisualization(_path);
            else
            {
                HidePath();
            }
        }

        public void Selected(bool isSelect)
        {
            _marker.SetActive(isSelect);
        }

        public void HidePath()
        {
            _PathDrawer.ClearPathVisualization();
        }

        public void HoverHighlight()
        {
        }

        public void ApproveMove()
        {
            if (_path != null)
            {
                _isNeedMove = true;
                IsOnAction = true;
                RunPath();
            }
        }

        public void StopMoving()
        {
        }

        public void LookAtNode(PathNode lookAtNode)
        {
            _targetNode = lookAtNode;
            if(!IsCountOrCostEnough())
                return;
            
            IsOnAction = true;
            UnitRotator.RotateToNext(lookAtNode);
        }

        private void DecreaseRotationCost()
        {
            _actionParams.ActionPoints -= _actionParams.moveParams.RotationCost;
        }
        
        
        private void DecreaseMoveCost()
        {
            var cost = _mathOperations.CalculateStepCost(_currentNode, _targetNode, _actionParams.moveParams.StepCost);
            _actionParams.ActionPoints -= cost;
        }
    }
}