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
        public List<Unit> Units { get; private set; } = new List<Unit>();

        private void OnEnable()
        {
            _gridManager.OnCreateGrid += GridManagerOnOnCreateGrid;
        }

        private void GridManagerOnOnCreateGrid()
        {
            for (int i = 0; i < _moverCount; i++)
            {
                Unit unit = Instantiate(_pathMoverPrefab, transform);
                var initialMode = GetRandomValue();
                UnitStats unitStats = new UnitStats(40, 100);
                unit.Init(pathFinder, initialMode, unitStats);
                initialMode.UnitCurrent = unit;
                unit.name += i;
                unit.transform.position = initialMode.GridPosition;
                Units.Add(unit);
            }
            
            OnSpawnEnd?.Invoke();
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