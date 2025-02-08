using System.Collections.Generic;
using System.Linq;
using Extra;
using UnityEngine;

namespace TacticalBattle
{
    public class PathFinder1 : MonoBehaviour
    {
        [SerializeField] private GridManager gridManager;
        [SerializeField] private int maxIterations = 1000;
        private MathOperations _mathOperations;

        private void Awake()
        {
            _mathOperations = new MathOperations();
        }

        public List<PathNode> FindPath(PathNode startNode, PathNode targetNode, UnitActionParams unitActionParams,
            float startRotation)
        {
            var rotationCost = unitActionParams.moveParams.RotationCost;
            var stepCost = unitActionParams.moveParams.StepCost;
            var startNodePos = startNode.GridPosition;
            var targetNodePos = targetNode.GridPosition;
            var unitRotation = startRotation;

            if (startNode == null || targetNode == null)
            {
                Debug.LogWarning("Невозможно найти путь: недопустимые начальная или конечная точки");
                return null;
            }

            gridManager.ResetAllNodes();

            var openSet = new HashSet<PathNode>();
            var closedSet = new HashSet<PathNode>();

            startNode.H = GetHeuristicDistance(startNodePos, targetNodePos);
            startNode.G = 0;
            openSet.Add(startNode);

            int iterations = 0;
            while (openSet.Count > 0 && iterations < maxIterations)
            {
                iterations++;

                PathNode current = openSet.OrderBy(x => x.F).First();

                if (current.GridPosition == targetNodePos)
                {
                    return ReconstrucePath(current);
                }

                openSet.Remove(current);
                closedSet.Add(current);

                foreach (var neighbor in gridManager.GetNeighbors(current))
                {
                    if (closedSet.Contains(neighbor))
                        continue;

                    // **Вычисляем стоимость поворота, учитывая текущий угол**
                    float rotationChange = _mathOperations.CalculateRotationCount(unitRotation, current.GridPosition,
                        neighbor.GridPosition);
                    float rotCost = rotationChange * rotationCost;

                    float baseMoveCost = GetTerrainMoveCost(neighbor.MoveType, stepCost);
                    bool isDiagonalDirection =
                        _mathOperations.IsDiagonalDirection(current.GridPosition, neighbor.GridPosition);
                    float moveCost = isDiagonalDirection ? baseMoveCost * 1.5f : baseMoveCost;

                    float totalCost = current.G + moveCost + rotCost;
                    if (totalCost > unitActionParams.ActionPoints)
                        continue;

                    if (!openSet.Contains(neighbor))
                    {
                        neighbor.G = totalCost;
                        neighbor.H = GetHeuristicDistance(neighbor.GridPosition, targetNodePos);
                        neighbor.Parent = current;

                        // **Обновляем угол поворота юнита на новом узле**
                        unitRotation = GetRotationByDirection(neighbor, current);

                        openSet.Add(neighbor);
                    }
                    else if (totalCost < neighbor.G)
                    {
                        neighbor.G = totalCost;
                        neighbor.Parent = current;

                        // **Обновляем угол только если нашли более выгодный путь**
                        unitRotation = GetRotationByDirection(neighbor, current);
                    }
                }
            }

            return null;
        }

        private float GetRotationByDirection(PathNode neighbor, PathNode current)
        {
            return Mathf.Atan2(neighbor.GridPosition.x - current.GridPosition.x,
                neighbor.GridPosition.z - current.GridPosition.z) * Mathf.Rad2Deg;
        }

        private float GetTerrainMoveCost(ETileMoveType tileType, float baseStepCost)
        {
            return tileType switch
            {
                ETileMoveType.Ground => baseStepCost,
                ETileMoveType.Grass => baseStepCost * 2f,
                ETileMoveType.Water => float.MaxValue,
                _ => baseStepCost
            };
        }

        private float GetHeuristicDistance(Vector3Int a, Vector3Int b)
        {
            Vector3Int diff = a - b;
            return Mathf.Max(Mathf.Abs(diff.x), Mathf.Abs(diff.z));
        }

        private List<PathNode> ReconstrucePath(PathNode endNode)
        {
            var path = new List<PathNode>();
            var current = endNode;

            while (current != null)
            {
                path.Add(current);
                current = current.Parent;
            }

            path.Reverse();
            return path;
        }
    }
}