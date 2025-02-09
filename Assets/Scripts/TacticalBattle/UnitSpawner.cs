using System;
using System.Collections.Generic;
using Data.Scripts;
using UnityEngine;
using Random = System.Random;

namespace TacticalBattle
{
    public class UnitSpawner : MonoBehaviour
    {
        [SerializeField] private Unit _pathMoverPrefab;
        [SerializeField] private int _moverCount;
        [SerializeField] private GridManager _gridManager;
        [SerializeField] private PathFinder pathFinder;

        public event Action OnSpawnEnd;
        private List<Unit> _units = new List<Unit>();

        private void OnEnable()
        {
            _gridManager.OnCreateGrid += GridManagerOnOnCreateGrid;
        }

        private void GridManagerOnOnCreateGrid()
        {
            for (int i = 0; i < _moverCount; i++)
            {
                Unit unit = Instantiate(_pathMoverPrefab, transform);
                UnitStats unitStats = new UnitStats(40, 100);
                var initialNode = GetRandomValue();
                unit.Init(pathFinder, initialNode,GetRandomAngle(), unitStats);
                initialNode.UnitCurrent = unit;
                unit.name += i;
                _units.Add(unit);
            }
            
            OnSpawnEnd?.Invoke();
        }

        public float GetRandomAngle()
        {
            int randomIndex = UnityEngine.Random.Range(0, 8);
            float angle = randomIndex * 45f;

            return angle;
        }
        private PathNode GetRandomValue()
        {
            Random random = new Random();
            List<PathNode> values = new List<PathNode>(_gridManager.Grid.Values);
            PathNode node = null;

            while (node == null)
            {
                int randomIndex = random.Next(values.Count);
                node = values[randomIndex];

                if (node.IsOcupied || node.MoveType == ETileMoveType.Water)
                {
                    node = null; 
                }
            }

            return node;
        }
    }
}