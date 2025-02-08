using System;
using System.Collections.Generic;
using Extra;
using UnityEngine;

namespace TacticalBattle
{
    public class PathFinder : MonoBehaviour
    {
        [SerializeField] private GridManager gridManager;
        [SerializeField] private int maxIterations = 1000;
        private MathOperations _mathOperations;
        private void Awake()
        {
            _mathOperations = new MathOperations();
        }

        public List<PathNode> FindPath(PathNode startNode, PathNode targetNode, float actionPoints, float startRotation)
        {
            gridManager.ResetAllNodes();

            var openList = new List<PathNode>();
            var closedList = new HashSet<PathNode>();

            startNode.G = 0;
            startNode.H = CalculateH(startNode, targetNode);
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

                var neighbors = gridManager.GetNeighbors(currentNode);

                foreach (var neighbor in neighbors)
                {
                    if (closedList.Contains(neighbor) || neighbor.IsOcupied)
                        continue;

                    float stepCost = _mathOperations.CalculateStepCost(currentNode, neighbor, Constants.STEP_COST);
                    float rotationCost = CalculateRotationCost(currentNode, neighbor, Constants.ROTATION_COST, currentRotation);
                    //float stepCost2 = CalculateRotationCost(startNode, targetNode, unitActionParams, currentRotation);

                    // Рассчитываем новый G, H и F
                    float newG = currentNode.G + stepCost + rotationCost;
                    float newH = CalculateH(neighbor, targetNode);
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
        
        private float CalculateRotationCost(PathNode currentNode, PathNode neighbor, float rotationCost,
            float startRotation)
        {
            // Если текущий узел не имеет родителя, то передаем начальный угол
            float currentRotation;
            if (currentNode.Parent == null)
            {
                // Это первая клетка, используем стартовый угол
                currentRotation = Mathf.RoundToInt(Mathf.DeltaAngle(0, startRotation));
            }
            else
            {
                // Если родитель существует, вычисляем угол относительно родителя
                var parentPosition = currentNode.Parent.GridPosition;
                Vector3 dirFromParent = currentNode.GridPosition - parentPosition;
                currentRotation = Mathf.Atan2(dirFromParent.x, dirFromParent.z) * Mathf.Rad2Deg;
            }
            
            //print(currentRotation);

            // Направление к следующей клетке
            Vector3 directionToNeighbor = neighbor.GridPosition - currentNode.GridPosition;
            float targetRotation = Mathf.Atan2(directionToNeighbor.x, directionToNeighbor.z) * Mathf.Rad2Deg;
            print(targetRotation);

            // Разница углов
            float angleDifference = Mathf.Abs(Mathf.DeltaAngle(currentRotation, targetRotation));

            // Рассчитываем стоимость поворота
            float totalRotationCost = Mathf.Abs(angleDifference / 45) * rotationCost;

            return totalRotationCost;
        }

        private float CalculateH(PathNode currentNode, PathNode targetNode)
        {
            return Mathf.Abs(currentNode.GridPosition.x - targetNode.GridPosition.x) +
                   Mathf.Abs(currentNode.GridPosition.z - targetNode.GridPosition.z);
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