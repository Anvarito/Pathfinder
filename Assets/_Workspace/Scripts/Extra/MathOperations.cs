using _Workspace.Scripts.TacticalBattle;
using UnityEngine;

namespace Extra
{
    public class MathOperations
    {
        public bool IsDiagonalDirection(Vector3Int from, Vector3Int to)
        {
            Vector3Int direction = to - from;
            bool isDiagonal = Mathf.Abs(direction.x) == 1 && Mathf.Abs(direction.z) == 1;
            return isDiagonal;
        }
        
        public int CalculateRotationCount(float yRotation, PathNode from, PathNode to)
        {
            int currentRotation = Mathf.RoundToInt(Mathf.DeltaAngle(0, yRotation));
            
            var angleDifference = GetAngleDifference(from, to, currentRotation);
            return Mathf.Abs(angleDifference / 45);
        }

        public float CalculateRotationCost(PathNode currentNode, PathNode targetNode, float startRotation)
        {
            float currentRotation;
            if (currentNode.Parent == null)
            {
                currentRotation = Mathf.RoundToInt(Mathf.DeltaAngle(0, startRotation));
            }
            else
            {
                var parentPosition = currentNode.Parent.GridPosition;
                Vector3 dirFromParent = currentNode.GridPosition - parentPosition;
                currentRotation = Mathf.Atan2(dirFromParent.x, dirFromParent.z) * Mathf.Rad2Deg;
            }
            
            var angleDifference = GetAngleDifference(currentNode, targetNode, currentRotation);
            return Mathf.Abs(angleDifference / 45);
        }

        
        private int GetAngleDifference(PathNode from, PathNode to, float currentRotation)
        {
            int targetRotation = GetRotation(from, to);
            int angleDifference = (int)Mathf.DeltaAngle(currentRotation, targetRotation);
            return angleDifference;
        }

        public int GetRotation(PathNode from, PathNode to)
        {
            Vector3 targetDirection = to.GridPosition - from.GridPosition;
            return Mathf.RoundToInt(Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg);
        }
        
        public float GetTerrainMoveCost(ETileMoveType tileType, float baseStepCost)
        {
            return tileType switch
            {
                ETileMoveType.Ground => baseStepCost,
                ETileMoveType.Grass => baseStepCost * 2f,
                ETileMoveType.Water => float.MaxValue,
                _ => baseStepCost
            };
        }
        
        public float CalculateStepCost(PathNode currentNode, PathNode targetNode, float baseStepCost)
        {
            float cost = GetTerrainMoveCost(targetNode.MoveType, baseStepCost);
            bool isDiagonal = IsDiagonalDirection(currentNode.GridPosition, targetNode.GridPosition);
            
            if (isDiagonal)
                cost *= Constants.DIAGONAL_MULTIPLIER; // Диагональ

            return cost;
        }
        
        public float CalculateH(PathNode currentNode, PathNode targetNode)
        {
            return Mathf.Abs(currentNode.GridPosition.x - targetNode.GridPosition.x) +
                   Mathf.Abs(currentNode.GridPosition.z - targetNode.GridPosition.z);
        }
    }
}