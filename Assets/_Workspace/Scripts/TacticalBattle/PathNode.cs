using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace _Workspace.Scripts.TacticalBattle
{
    public enum ETileMoveType
    {
        Ground,
        Water,
        Grass
    }

    public class PathNode : MonoBehaviour
    {
        [SerializeField] private ETileMoveType _moveType;
        public TextMeshProUGUI _Text;

        public Wall WallPrefab;

        // Словарь для хранения стен по направлениям
        private Dictionary<Wall.WallDirection, Wall> _walls = new Dictionary<Wall.WallDirection, Wall>();
        public Unit UnitCurrent { get; set; }

        public Vector3Int GridPosition
        {
            get => new Vector3Int(
                Mathf.RoundToInt(transform.position.x),
                Mathf.RoundToInt(transform.position.y),
                Mathf.RoundToInt(transform.position.z)
            );
        }

        public bool IsOcupied => UnitCurrent;

        public ETileMoveType MoveType => _moveType;
        public float G { get; set; }
        public float H { get; set; }
        public float F => G + H;
        public PathNode Parent { get; set; }


        // Проверяем есть ли стена в указанном направлении
        public bool HasWall(Wall.WallDirection direction) => _walls.ContainsKey(direction);


        public void Reset()
        {
            G = 0;
            H = 0;
            Parent = null;
        }

        public void FindWalls()
        {
            // Находим все стены в дочерних объектах
            foreach (var wall in GetComponentsInChildren<Wall>())
            {
                AddWall(wall);
            }
        }

        public void AddWall(Wall wall)
        {
            wall.OnRemove += () =>
            {
                _walls.Remove(wall.Direction);
            };
            
            _walls[wall.Direction] = wall;
            wall.RotateByDirection();
        }
    }
}