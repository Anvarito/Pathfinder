using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Workspace.Scripts.TacticalBattle
{
    public class GridManager : MonoBehaviour
    {
        [Header("Debug Visualization")] [SerializeField]
        private bool _showConnections = true;

        [SerializeField] private Color _connectionColor = new Color(0.2f, 0.8f, 0.2f, 0.4f);
        [SerializeField] private float _connectionHeight = 0.1f;

        public Dictionary<Vector3Int, PathNode> Grid { get; private set; } = new Dictionary<Vector3Int, PathNode>();
        public event Action OnCreateGrid;

        private void Awake()
        {
            foreach (PathNode pathNode in GetComponentsInChildren<PathNode>())
            {
                // Корректируем позицию
                Vector3 position = pathNode.transform.position;
                Vector3 correctedPosition = new Vector3(
                    Mathf.Round(position.x),
                    0f, // Принудительно устанавливаем Y в 0
                    Mathf.Round(position.z)
                );

                // Применяем скорректированную позицию
                if (position != correctedPosition)
                {
                    pathNode.transform.position = correctedPosition;
                    Debug.Log($"Corrected position for {pathNode.name} from {position} to {correctedPosition}");
                }

                // Проверяем, нет ли уже ноды в этой позиции
                if (Grid.TryGetValue(pathNode.GridPosition, out PathNode existingNode))
                {
                    Destroy(pathNode.gameObject);
                    continue;
                }

                pathNode.FindWalls();
                Grid[pathNode.GridPosition] = pathNode;
                pathNode.name = $"PathNode {pathNode.GridPosition.x}-{pathNode.GridPosition.z}";

                var result = "";
                foreach (char c in pathNode.name)
                {
                    if (Char.IsDigit(c) || c == '-')
                    {
                        result += c;
                    }
                }

                pathNode._Text.text = result;
            }

            OnCreateGrid?.Invoke();
        }

        [ContextMenu("Randomize obstacles")]
        private void DEBUGrandomizingObstackles()
        {
            foreach (var pair in Grid)
            {
                //pair.Value.RandomingMoveType();
            }
        }

        public void ResetAllNodes()
        {
            foreach (var pair in Grid)
            {
                pair.Value.Reset();
            }
        }

        public PathNode GetNode(Vector3Int position)
        {
            Grid.TryGetValue(position, out PathNode node);
            return node;
        }

        public List<PathNode> GetNeighbors(PathNode node)
        {
            var neighbors = new List<PathNode>();

            foreach (Directions dir in Enum.GetValues(typeof(Directions)))
            {
                var direction = GetDirectionVector(dir);
                var neighborPos = node.GridPosition + direction;

                if (!Grid.TryGetValue(neighborPos, out PathNode neighbor) || neighbor == null ||
                    neighbor.MoveType == ETileMoveType.Water)
                    continue;

                // Проверяем возможность перемещения с учетом стен
                if (CanMove(node, neighbor, dir))
                {
                    neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }

        private bool CanMove(PathNode from, PathNode to, Directions direction)
        {
            if (to.MoveType == ETileMoveType.Water)
                return false;

            // Для прямого движения проверяем только стены между тайлами
            switch (direction)
            {
                case Directions.Forward:
                    return !from.HasWall(Wall.WallDirection.North) &&
                           !to.HasWall(Wall.WallDirection.South)
                           && !to.IsOcupied;

                case Directions.Right:
                    return !from.HasWall(Wall.WallDirection.East) &&
                           !to.HasWall(Wall.WallDirection.West)
                           && !to.IsOcupied;

                case Directions.Back:
                    return !from.HasWall(Wall.WallDirection.South) &&
                           !to.HasWall(Wall.WallDirection.North)
                           && !to.IsOcupied;

                case Directions.Left:
                    return !from.HasWall(Wall.WallDirection.West) &&
                           !to.HasWall(Wall.WallDirection.East)
                           && !to.IsOcupied;
            }

            // Для диагонального движения проверяем возможность прохода по обоим прямым путям
            Vector3Int intermediatePos1, intermediatePos2;
            switch (direction)
            {
                case Directions.ForwardRight:
                    intermediatePos1 = new Vector3Int(from.GridPosition.x + 1, 0, from.GridPosition.z);
                    intermediatePos2 = new Vector3Int(from.GridPosition.x, 0, from.GridPosition.z + 1);
                    return CanMoveStraight(from, intermediatePos1, Directions.Right) &&
                           CanMoveStraight(from, intermediatePos2, Directions.Forward) &&
                           CanMoveStraight(intermediatePos1, to, Directions.Forward) &&
                           CanMoveStraight(intermediatePos2, to, Directions.Right)
                           && !to.IsOcupied;

                case Directions.BackRight:
                    intermediatePos1 = new Vector3Int(from.GridPosition.x + 1, 0, from.GridPosition.z);
                    intermediatePos2 = new Vector3Int(from.GridPosition.x, 0, from.GridPosition.z - 1);
                    return CanMoveStraight(from, intermediatePos1, Directions.Right) &&
                           CanMoveStraight(from, intermediatePos2, Directions.Back) &&
                           CanMoveStraight(intermediatePos1, to, Directions.Back) &&
                           CanMoveStraight(intermediatePos2, to, Directions.Right)
                           && !to.IsOcupied;

                case Directions.BackLeft:
                    intermediatePos1 = new Vector3Int(from.GridPosition.x - 1, 0, from.GridPosition.z);
                    intermediatePos2 = new Vector3Int(from.GridPosition.x, 0, from.GridPosition.z - 1);
                    return CanMoveStraight(from, intermediatePos1, Directions.Left) &&
                           CanMoveStraight(from, intermediatePos2, Directions.Back) &&
                           CanMoveStraight(intermediatePos1, to, Directions.Back) &&
                           CanMoveStraight(intermediatePos2, to, Directions.Left)
                           && !to.IsOcupied;

                case Directions.ForwardLeft:
                    intermediatePos1 = new Vector3Int(from.GridPosition.x - 1, 0, from.GridPosition.z);
                    intermediatePos2 = new Vector3Int(from.GridPosition.x, 0, from.GridPosition.z + 1);
                    return CanMoveStraight(from, intermediatePos1, Directions.Left) &&
                           CanMoveStraight(from, intermediatePos2, Directions.Forward) &&
                           CanMoveStraight(intermediatePos1, to, Directions.Forward) &&
                           CanMoveStraight(intermediatePos2, to, Directions.Left)
                           && !to.IsOcupied;
            }

            return false;
        }

        private bool CanMoveStraight(PathNode from, Vector3Int toPos, Directions direction)
        {
            if (!Grid.TryGetValue(toPos, out PathNode to) || to.MoveType == ETileMoveType.Water)
                return false;

            switch (direction)
            {
                case Directions.Forward:
                    return !from.HasWall(Wall.WallDirection.North) &&
                           !to.HasWall(Wall.WallDirection.South);

                case Directions.Right:
                    return !from.HasWall(Wall.WallDirection.East) &&
                           !to.HasWall(Wall.WallDirection.West);

                case Directions.Back:
                    return !from.HasWall(Wall.WallDirection.South) &&
                           !to.HasWall(Wall.WallDirection.North);

                case Directions.Left:
                    return !from.HasWall(Wall.WallDirection.West) &&
                           !to.HasWall(Wall.WallDirection.East);

                default:
                    return false;
            }
        }

        private bool CanMoveStraight(Vector3Int fromPos, PathNode to, Directions direction)
        {
            if (!Grid.TryGetValue(fromPos, out PathNode from) || from.MoveType == ETileMoveType.Water)
                return false;

            return CanMoveStraight(from, to.GridPosition, direction);
        }

        public enum Directions
        {
            Forward, // Вперед
            ForwardRight, // Вперед-вправо
            Right, // Вправо
            BackRight, // Назад-вправо
            Back, // Назад
            BackLeft, // Назад-влево
            Left, // Влево
            ForwardLeft // Вперед-влево
        }

        public Vector3Int GetDirectionVector(Directions direction)
        {
            switch (direction)
            {
                case Directions.Forward:
                    return new Vector3Int(0, 0, 1);
                case Directions.ForwardRight:
                    return new Vector3Int(1, 0, 1);
                case Directions.Right:
                    return new Vector3Int(1, 0, 0);
                case Directions.BackRight:
                    return new Vector3Int(1, 0, -1);
                case Directions.Back:
                    return new Vector3Int(0, 0, -1);
                case Directions.BackLeft:
                    return new Vector3Int(-1, 0, -1);
                case Directions.Left:
                    return new Vector3Int(-1, 0, 0);
                case Directions.ForwardLeft:
                    return new Vector3Int(-1, 0, 1);
                default:
                    return Vector3Int.zero; // Если почему-то значение не совпадает
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!_showConnections || Grid == null)
                return;

            Gizmos.color = _connectionColor;

            foreach (var node in Grid.Values)
            {
                if (node == null || node.MoveType == ETileMoveType.Water)
                    continue;

                // Используем центры тайлов для более точной визуализации
                Vector3 startCenter = node.transform.position + Vector3.up * _connectionHeight;

                foreach (var neighbor in GetNeighbors(node))
                {
                    // Проверяем, что рисуем линию только в одном направлении
                    if (ShouldDrawConnection(node.GridPosition, neighbor.GridPosition))
                    {
                        Vector3 endCenter = neighbor.transform.position + Vector3.up * _connectionHeight;

                        // Для диагональных связей добавляем промежуточную точку
                        if (Mathf.Abs(neighbor.GridPosition.x - node.GridPosition.x) == 1 &&
                            Mathf.Abs(neighbor.GridPosition.z - node.GridPosition.z) == 1)
                        {
                            // Создаем промежуточную точку для более четкой визуализации
                            Vector3 midPoint = (startCenter + endCenter) * 0.5f;
                            Gizmos.DrawLine(startCenter, midPoint);
                            Gizmos.DrawLine(midPoint, endCenter);
                        }
                        else
                        {
                            Gizmos.DrawLine(startCenter, endCenter);
                        }
                    }
                }
            }
        }

        private bool ShouldDrawConnection(Vector3Int from, Vector3Int to)
        {
            // Рисуем связь только в одном направлении
            if (to.x > from.x) return true;
            if (to.x < from.x) return false;
            return to.z > from.z;
        }
#endif
    }
}