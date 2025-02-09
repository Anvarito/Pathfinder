using System;
using UnityEngine;

namespace _Workspace.Scripts.TacticalBattle
{
    public class Wall : MonoBehaviour
    {
        public enum WallDirection
        {
            North,  // Перегородка сверху (Z+)
            East,   // Перегородка справа (X+)
            South,  // Перегородка снизу (Z-)
            West    // Перегородка слева (X-)
        }

        [SerializeField] private WallDirection _direction;
        public WallDirection Direction => _direction;
        public event Action OnRemove;
        private void OnValidate()
        {
            // Поворачиваем стену в зависимости от направления
            RotateByDirection();
        }

        private void OnDestroy()
        {
            OnRemove?.Invoke();
        }

        public void RotateByDirection()
        {
            float rotation = _direction switch
            {
                WallDirection.North => 0,
                WallDirection.East => 90,
                WallDirection.South => 180,
                WallDirection.West => 270,
                _ => 0
            };

            Vector3 position = _direction switch
            {
                WallDirection.North => new Vector3(0, 0, 0.5f),
                WallDirection.East => new Vector3(0.5f, 0, 0),
                WallDirection.South => new Vector3(0, 0, -0.5f),
                WallDirection.West => new Vector3(-0.5f, 0, 0)
            };

            transform.localPosition = position;
            transform.localRotation = Quaternion.Euler(0, rotation, 0);
        }
    }
}