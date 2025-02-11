using System;
using _Workspace.Scripts.TacticalBattle.HUD;
using UnityEngine;
using Zenject;

namespace _Workspace.Scripts.TacticalBattle
{
    public class TacticalBattleTurnController : IInitializable, IDisposable
    {
        private readonly IUnitSpawner _unitSpawner;
        private readonly MainButtonListener _mainButtonListener;

        public TacticalBattleTurnController(IUnitSpawner spawner, MainButtonListener mainButtonListener)
        {
            _unitSpawner = spawner;
            _mainButtonListener = mainButtonListener;
        }

        public void Initialize()
        {
            _mainButtonListener.OnTurnEndPress += OnTurnEndPress;
            _mainButtonListener.OnClearPress += Clear;
            _mainButtonListener.OnLoadPress += Load;
            _mainButtonListener.OnSavePress += Save;
            _mainButtonListener.OnSpawnOnePress += SpawnOne;
        }
        private void SpawnOne()
        {
            _unitSpawner.Spawn();
        }

        private void Clear()
        {
            _unitSpawner.Clear();
        }

        private void Load()
        {
            _unitSpawner.Load();
        }

        private void Save()
        {
            _unitSpawner.Save();
        }

        private void OnTurnEndPress()
        {
            foreach (var unit in _unitSpawner.Units)
            {
                unit.Stats.RestoreActionPoints();
            }
        }

        public void Dispose()
        {
            _mainButtonListener.OnTurnEndPress -= OnTurnEndPress;
            _mainButtonListener.OnClearPress -= Clear;
            _mainButtonListener.OnLoadPress -= Load;
            _mainButtonListener.OnSavePress -= Save;
            _mainButtonListener.OnSpawnOnePress -= SpawnOne;
        }
    }
}