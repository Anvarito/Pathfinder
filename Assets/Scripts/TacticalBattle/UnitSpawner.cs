using System;
using System.Collections.Generic;
using Data.Scripts;
using UnityEngine;
using Random = System.Random;
using TIM;

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
                // Unit unit = Instantiate(_pathMoverPrefab, transform);
                // UnitStats unitStats = new UnitStats(40, 100);
                // var initialNode = GetRandomValue();
                // unit.Init(pathFinder, initialNode, GetRandomAngle(), unitStats);
                // initialNode.UnitCurrent = unit;
                // unit.name += i;
                // _units.Add(unit);
            }

            OnSpawnEnd?.Invoke();
        }

        [ContextMenu("Load")]
        public void Load()
        {
            var datas = LoadUnitsData();
            for (int i = 0; i < datas.Count; i++)
            {
                UnitSaveData data;
                if (datas[i] == null)
                {
                    data = datas[0];
                }

                data = datas[i];
                Unit unit = Instantiate(_pathMoverPrefab, transform);
                UnitStats unitStats = new UnitStats(data.MaxActionPoints, data.MaxHitPoints, data.CurrentActionPoints,data.CurrentHitPoints);
                Vector3Int toInt = new Vector3Int((int)data.CurrentNode.x, (int)data.CurrentNode.y, (int)data.CurrentNode.z);
                var node = _gridManager.Grid.GetValueOrDefault(toInt);
                unit.Init(pathFinder, node,data.Rotation , unitStats);
                node.UnitCurrent = unit;
                unit.name = data.UnitName;
                _units.Add(unit);
            }
        }

        public List<UnitSaveData> LoadUnitsData()
        {
            SaveSystem.LoadAll();
            var data = SaveSystem.Get<List<UnitSaveData>>("Units");
            return data;
        }

        [ContextMenu("Save")]
        public void SaveData()
        {
            List<UnitSaveData> saveDatas = new List<UnitSaveData>();
            foreach (var unit in _units)
            {
                UnitSaveData unitSaveData =
                    new UnitSaveData(unit.name, unit.CurrentNOde, unit.transform.position, unit.transform.eulerAngles,
                        unit.ActionPoints.CurrentActionPoints.value, unit.ActionPoints.MaxActionPoints,
                        unit.HitPoints.MaxHitPoints, unit.HitPoints.CurrentHitPoints.value);
                saveDatas.Add(unitSaveData);
            }

            SaveSystem.Set(saveDatas, "Units");
            SaveSystem.SaveAll();
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