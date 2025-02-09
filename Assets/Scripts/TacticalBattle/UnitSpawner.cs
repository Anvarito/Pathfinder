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
                Spawn();
            }

            OnSpawnEnd?.Invoke();
        }
        
        [ContextMenu("SpawnNew")]
        public void Spawn()
        {
            Unit unit = Instantiate(_pathMoverPrefab, transform);
            UnitStats unitStats = new UnitStats();
            var node = GetRandomNode();
            unit.Init(pathFinder, node, GetRandomAngle(), unitStats);
            node.UnitCurrent = unit;
            unit.name = UnityEngine.Random.Range(0, 1000).ToString();
            _units.Add(unit);
        }

        [ContextMenu("Load")]
        public void Load()
        {
            var datas = LoadUnitsData();
            for (int i = 0; i < datas.Count; i++)
            {
                UnitSaveData data = datas[i];
                if (data == null)
                {
                    Spawn();
                    Debug.Log("Data not loaded!");
                    return;
                }
                
                Unit unit = Instantiate(_pathMoverPrefab, transform);
                UnitStats unitStats = new UnitStats(data.UnitStatsSaveData);

                var pos = data.UnitTransformSaveData.NodePosition;
                var rot = data.UnitTransformSaveData.RotationDir;
                var node = _gridManager.Grid.GetValueOrDefault(pos);
                unit.Init(pathFinder, node, rot, unitStats);
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
                UnitSaveData unitSaveData = new UnitSaveData(unit.name, unit.GetSaveData(), unit.Stats.GetSaveData());
                saveDatas.Add(unitSaveData);
            }

            SaveSystem.Set(saveDatas, "Units");
            SaveSystem.SaveAll();
        }

        public Vector3 GetRandomAngle()
        {
            int randomIndex = UnityEngine.Random.Range(0, 8);
            float angle = randomIndex * 45f;
            Vector3 dir = new Vector3(0, angle, 0);
            return dir;
        }

        private PathNode GetRandomNode()
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