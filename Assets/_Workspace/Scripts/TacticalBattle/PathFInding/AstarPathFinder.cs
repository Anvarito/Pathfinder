using System.Collections.Generic;
using Extra;
using UnityEngine;

namespace _Workspace.Scripts.TacticalBattle.PathFInding
{
    public interface IPathFinder
    {
        public List<PathNode> FindPath(PathNode startNode, PathNode targetNode, float actionPoints,
            float startRotation);
    }
    public class AstarPathFinder : IPathFinder
    {
        private int maxIterations = 5000;
        private readonly MathOperations _mathOperations;
        private readonly GraphMaster _graphMaster;

        private AstarPathFinder(MathOperations mathOperations, GraphMaster graphMaster)
        {
            _mathOperations = mathOperations;
            _graphMaster = graphMaster;
        }

        public List<PathNode> FindPath(PathNode startNode, PathNode targetNode, float actionPoints, float startRotation)
        {
            _graphMaster.ResetAllNodes();

            var openList = new List<PathNode>();
            var closedList = new HashSet<PathNode>();

            startNode.G = 0;
            startNode.H = _mathOperations.CalculateH(startNode, targetNode);
            startNode.Parent = null;
            openList.Add(startNode);

            float currentRotation = startRotation; // Начальный угол поворота
            int iterations = 0;

            while (openList.Count > 0 && iterations < maxIterations)
            {
                iterations++;

                openList.Sort((a, b) => a.F.CompareTo(b.F));

                PathNode currentNode = openList[0];
                openList.RemoveAt(0);

                if (currentNode == targetNode)
                {
                    return ReconstructPath(currentNode);
                }

                closedList.Add(currentNode);

                var neighbors = _graphMaster.GetNeighbors(currentNode);

                foreach (var neighbor in neighbors)
                {
                    if (closedList.Contains(neighbor) || neighbor.IsOcupied)
                        continue;

                    float stepCost = _mathOperations.CalculateStepCost(currentNode, neighbor, Constants.STEP_COST);
                    float rotationCost = _mathOperations.CalculateRotationCost(currentNode, neighbor, currentRotation) * Constants.ROTATION_COST;
                    //float stepCost2 = CalculateRotationCost(startNode, targetNode, unitActionParams, currentRotation);

                    // Рассчитываем новый G, H и F
                    float newG = currentNode.G + stepCost + rotationCost;
                    float newH = _mathOperations.CalculateH(neighbor, targetNode);
                    //print($"{stepCost2} ");

                    // Проверка на доступные очки
                    if (newG > actionPoints)
                    {
                        continue; // Пропускаем, если общая стоимость (шаг + поворот) превышает очки
                    }

                    PathNode existingNeighbor = openList.Find(n => n == neighbor);
                    if (existingNeighbor == null || newG < neighbor.G)
                    {
                        neighbor.G = newG;
                        neighbor.H = newH;
                        neighbor.Parent = currentNode;

                        if (existingNeighbor == null)
                        {
                            openList.Add(neighbor);
                        }
                        else
                        {
                            openList.Remove(existingNeighbor);
                            openList.Add(neighbor);
                        }
                    }
                }
            }

            return null; // Путь не найден
        }
        
        

        private List<PathNode> ReconstructPath(PathNode targetNode)
        {
            var path = new List<PathNode>();
            PathNode currentNode = targetNode;

            while (currentNode != null)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }

            path.Reverse();
            return path;
        }
    }
}