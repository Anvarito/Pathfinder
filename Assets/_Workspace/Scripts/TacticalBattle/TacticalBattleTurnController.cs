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
            Debug.Log("PROC CONST");
        }

        public void Initialize()
        {
            Debug.Log("PROC INIT");
            _mainButtonListener.OnTurnEndPress += OnTurnEndPress;
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
        }
    }
}