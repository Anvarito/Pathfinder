using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

namespace TacticalBattle
{
    public class UnitSpawner : MonoBehaviour
    {
        [SerializeField] private Unit _pathMoverPrefab;
        [SerializeField] private int _moverCount;
        [SerializeField] private GridManager _gridManager;
        [SerializeField] private PathFinder pathFinder;

        private void OnEnable()
        {
            _gridManager.OnCreateGrid += GridManagerOnOnCreateGrid;
        }

        private void GridManagerOnOnCreateGrid()
        {
            for(int i = 0; i < _moverCount; i ++)
            {
                Unit unit = Instantiate(_pathMoverPrefab, transform);
                var node = GetRandomValue();
                unit.Init(pathFinder, node);
                node.UnitCurrent = unit;
                unit.name += i;
                unit.transform.position = node.GridPosition;
            }
        }
        
        public PathNode GetRandomValue()
        {
            // Используем Random для генерации индекса
            Random random = new Random();
            int randomIndex = random.Next(_gridManager.Grid.Count);  // Генерируем индекс от 0 до количества элементов в словаре - 1

            // Извлекаем список значений
            var values = new List<PathNode>(_gridManager.Grid.Values);
            return values[randomIndex]; // Возвращаем случайное значение
        }
    }
}