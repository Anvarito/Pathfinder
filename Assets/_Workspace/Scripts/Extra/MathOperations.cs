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
        
        public int CalculateRotationCount(float yRotation, Vector3 from, Vector3 to)
        {
            Vector3 targetDirection = to - from;
            int targetRotation = GetRotation(targetDirection);
            int currentRotation = Mathf.RoundToInt(Mathf.DeltaAngle(0, yRotation));
            int angleDiff = (int)Mathf.DeltaAngle(currentRotation, targetRotation);
            return Mathf.Abs(angleDiff / 45);
        }

        public int GetRotation(Vector3 targetDirection)
        {
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