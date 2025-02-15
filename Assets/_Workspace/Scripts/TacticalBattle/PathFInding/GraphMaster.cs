using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Workspace.Scripts.TacticalBattle.PathFInding
{
    public class GraphMaster : MonoBehaviour
    {
        [Header("Debug Visualization")] [SerializeField]
        private bool _showConnections = true;

        [SerializeField] private Color _connectionColor = new Color(0.2f, 0.8f, 0.2f, 0.4f);
        [SerializeField] private float _connectionHeight = 0.1f;

        public Dictionary<Vector3, PathNode> Grid { get; private set; } = new Dictionary<Vector3, PathNode>();
        public event Action OnCreateGraph;

        private void Awake()
        {
            foreach (PathNode pathNode in GetComponentsInChildren<PathNode>())
            {
                pathNode.FindWalls();
                Grid[pathNode.GridPosition] = pathNode;
                pathNode.name = $"PathNode {pathNode.GridPosition.x}-{pathNode.GridPosition.y}-{pathNode.GridPosition.z}";

                pathNode.Init();
            }

            OnCreateGraph?.Invoke();
        }

        public void ResetAllNodes()
        {
            foreach (var pair in Grid)
            {
                pair.Value.Reset();
            }
        }

        public PathNode GetNodeBy(Vector3 position)
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
                case Directions.ForwardUp:
                    return !from.HasWall(Wall.WallDirection.North) &&
                           !to.HasWall(Wall.WallDirection.South)
                           && !to.IsOcupied;

                case Directions.Right:
                case Directions.RightUp:
                    return !from.HasWall(Wall.WallDirection.East) &&
                           !to.HasWall(Wall.WallDirection.West)
                           && !to.IsOcupied;

                case Directions.Back:
                case Directions.BackUp:
                    return !from.HasWall(Wall.WallDirection.South) &&
                           !to.HasWall(Wall.WallDirection.North)
                           && !to.IsOcupied;

                case Directions.Left:
                case Directions.LeftUp:
                    return !from.HasWall(Wall.WallDirection.West) &&
                           !to.HasWall(Wall.WallDirection.East)
                           && !to.IsOcupied;
            }

            // Для диагонального движения проверяем возможность прохода по обоим прямым путям
            Vector3 intermediatePos1, intermediatePos2;
            switch (direction)
            {
                case Directions.ForwardRight:
                case Directions.ForwardRightUp:
                    intermediatePos1 = new Vector3(from.GridPosition.x + 1, from.GridPosition.y, from.GridPosition.z);
                    intermediatePos2 = new Vector3(from.GridPosition.x, from.GridPosition.y, from.GridPosition.z + 1);
                    return CanMoveStraight(from, intermediatePos1, Directions.Right) &&
                           CanMoveStraight(from, intermediatePos2, Directions.Forward) &&
                           CanMoveStraight(intermediatePos1, to, Directions.Forward) &&
                           CanMoveStraight(intermediatePos2, to, Directions.Right)
                           && !to.IsOcupied;

                case Directions.BackRight:
                case Directions.BackRightUp:
                    intermediatePos1 = new Vector3(from.GridPosition.x + 1, from.GridPosition.y, from.GridPosition.z);
                    intermediatePos2 = new Vector3(from.GridPosition.x, from.GridPosition.y, from.GridPosition.z - 1);
                    return CanMoveStraight(from, intermediatePos1, Directions.Right) &&
                           CanMoveStraight(from, intermediatePos2, Directions.Back) &&
                           CanMoveStraight(intermediatePos1, to, Directions.Back) &&
                           CanMoveStraight(intermediatePos2, to, Directions.Right)
                           && !to.IsOcupied;

                case Directions.BackLeft:
                case Directions.BackLeftUp:
                    intermediatePos1 = new Vector3(from.GridPosition.x - 1, from.GridPosition.y, from.GridPosition.z);
                    intermediatePos2 = new Vector3(from.GridPosition.x, from.GridPosition.y, from.GridPosition.z - 1);
                    return CanMoveStraight(from, intermediatePos1, Directions.Left) &&
                           CanMoveStraight(from, intermediatePos2, Directions.Back) &&
                           CanMoveStraight(intermediatePos1, to, Directions.Back) &&
                           CanMoveStraight(intermediatePos2, to, Directions.Left)
                           && !to.IsOcupied;

                case Directions.ForwardLeft:
                case Directions.ForwardLeftUp:
                    intermediatePos1 = new Vector3(from.GridPosition.x - 1, from.GridPosition.y, from.GridPosition.z);
                    intermediatePos2 = new Vector3(from.GridPosition.x, from.GridPosition.y, from.GridPosition.z + 1);
                    return CanMoveStraight(from, intermediatePos1, Directions.Left) &&
                           CanMoveStraight(from, intermediatePos2, Directions.Forward) &&
                           CanMoveStraight(intermediatePos1, to, Directions.Forward) &&
                           CanMoveStraight(intermediatePos2, to, Directions.Left)
                           && !to.IsOcupied;
            }

            return false;
        }

        private bool CanMoveStraight(PathNode from, Vector3 toPos, Directions direction)
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

        private bool CanMoveStraight(Vector3 fromPos, PathNode to, Directions direction)
        {
            if (!Grid.TryGetValue(fromPos, out PathNode from) || from.MoveType == ETileMoveType.Water)
                return false;

            return CanMoveStraight(from, to.GridPosition, direction);
        }

        public enum Directions
        {
            Forward, // Вперед
            ForwardUp, // ВпередВверх
            ForwardRight, // Вперед-вправо
            ForwardRightUp, // ВпередВправоВверх
            Right, // Вправо
            RightUp, // ВправоВверх
            BackRight, // Назад-вправо
            BackRightUp, // Назад-вправоВверх
            Back, // Назад
            BackUp, // НазадВверх
            BackLeft, // Назад-влево
            BackLeftUp, // Назад-влевоВверх
            Left, // Влево
            LeftUp, // ВлевоВверх
            ForwardLeft, // Вперед-влево
            ForwardLeftUp // Вперед-влевоВверх
        }

        public Vector3 GetDirectionVector(Directions direction)
        {
            switch (direction)
            {
                case Directions.Forward:
                    return new Vector3(0, 0, 1);
                case Directions.ForwardUp:
                    return new Vector3(0, 0.5f, 1);
                case Directions.ForwardRight:
                    return new Vector3(1, 0, 1);
                case Directions.ForwardRightUp:
                    return new Vector3(1, 0.5f, 1);
                case Directions.Right:
                    return new Vector3(1, 0, 0);
                case Directions.RightUp:
                    return new Vector3(1, 0.5f, 0);
                case Directions.BackRight:
                    return new Vector3(1, 0, -1);
                case Directions.BackRightUp:
                    return new Vector3(1, 0.5f, -1);
                case Directions.Back:
                    return new Vector3(0, 0, -1);
                case Directions.BackUp:
                    return new Vector3(0, 0.5f, -1);
                case Directions.BackLeft:
                    return new Vector3(-1, 0, -1);
                case Directions.BackLeftUp:
                    return new Vector3(-1, 0.5f, -1);
                case Directions.Left:
                    return new Vector3(-1, 0, 0);
                case Directions.LeftUp:
                    return new Vector3(-1, 0.5f, 0);
                case Directions.ForwardLeft:
                    return new Vector3(-1, 0, 1);
                case Directions.ForwardLeftUp:
                    return new Vector3(-1, 0.5f, 1);
                default:
                    return Vector3.zero;
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

        private bool ShouldDrawConnection(Vector3 from, Vector3 to)
        {
            // Рисуем связь только в одном направлении
            if (to.x > from.x) return true;
            if (to.x < from.x) return false;
            return to.z > from.z;
        }
#endif
    }
}