using System;
using System.Collections.Generic;
using _Workspace.Scripts.Data.Scripts;
using _Workspace.Scripts.SaveLoad;
using Extra;
using UnityEngine;
using Zenject;

namespace _Workspace.Scripts.TacticalBattle
{
    public interface IUnitSpawner : IUnitListHolder
    {
        public void Spawn();
    }
    public interface IUnitListHolder
    {
        public List<Unit> Units { get; }
    }
    public class UnitSpawner : MonoBehaviour, IUnitSpawner
    {
        [SerializeField] private Unit _pathMoverPrefab;
        [SerializeField] private int _moverCount;
        [SerializeField] private GridManager _gridManager;
        [SerializeField] private PathFinder pathFinder;

        public event Action OnSpawnEnd;
        public List<Unit> Units { get; private set; } = new List<Unit>();
        private ISaveLoaderBattleUnits _saveLoaderBattleUnits;
        private MathOperations _mathOperations;

        [Inject]
        public void Construct(ISaveLoaderBattleUnits saveLoaderBattleUnits, MathOperations mathOperations)
        {
            _saveLoaderBattleUnits = saveLoaderBattleUnits;
            _mathOperations = mathOperations;
        }

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
            unit.Init(pathFinder, node, GetRandomAngle(), _mathOperations, unitStats);
            node.UnitCurrent = unit;
            unit.name = UnityEngine.Random.Range(0, 1000).ToString();
            Units.Add(unit);
        }

        [ContextMenu("Load")]
        private void Load()
        {
            var datas = _saveLoaderBattleUnits.LoadUnitsData();
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
                unit.Init(pathFinder, node, rot, _mathOperations, unitStats);
                node.UnitCurrent = unit;
                unit.name = data.UnitName;
                Units.Add(unit);
            }
        }


        [ContextMenu("Save")]
        private void Save()
        {
            _saveLoaderBattleUnits.SaveData(Units);
        }

        private Vector3 GetRandomAngle()
        {
            int randomIndex = UnityEngine.Random.Range(0, 8);
            float angle = randomIndex * 45f;
            Vector3 dir = new Vector3(0, angle, 0);
            return dir;
        }

        private PathNode GetRandomNode()
        {
            System.Random random = new System.Random();
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