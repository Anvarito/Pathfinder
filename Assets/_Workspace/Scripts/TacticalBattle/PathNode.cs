using System;
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
        private MeshRenderer _meshRenderer;
        public Transform _tileHelper;
        public bool IsNeedHelper = false;
        public readonly string _slideParam = "_Slide";

        public Wall WallPrefab;

        // Словарь для хранения стен по направлениям
        private Dictionary<Wall.WallDirection, Wall> _walls = new Dictionary<Wall.WallDirection, Wall>();
        public Unit UnitCurrent { get; set; }

        public Vector3 GridPosition
        {
            get => new Vector3(
                Mathf.RoundToInt(transform.position.x),
                transform.position.y,
                Mathf.RoundToInt(transform.position.z)
            );
        }

        public bool IsOcupied => UnitCurrent;

        public ETileMoveType MoveType => _moveType;
        public float G { get; set; }
        public float H { get; set; }
        public float F => G + H;
        public PathNode Parent { get; set; }
        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            Material material = new Material(_meshRenderer.sharedMaterial);
            _meshRenderer.sharedMaterial = material;
        }



        // Проверяем есть ли стена в указанном направлении
        public bool HasWall(Wall.WallDirection direction) => _walls.ContainsKey(direction);

        public void Init()
        {
            if(!IsNeedHelper)
                return;
            var result = "";
            foreach (char c in gameObject.name)
            {
                if (Char.IsDigit(c) || c == '-')
                {
                    result += c;
                }
            }
            
            
            var hepler = Instantiate(_tileHelper, transform);
            hepler.GetComponentInChildren<TextMeshProUGUI>().text = result;
        }

        public void Reset()
        {
            G = 0;
            H = 0;
            Parent = null;
        }

        public void SetNodeAsWalkableColor()
        {
            _meshRenderer.sharedMaterial.SetFloat(_slideParam, 1);
        }

        public void SetNodeAsNormalColor()
        {
            _meshRenderer.sharedMaterial.SetFloat(_slideParam, 0);
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