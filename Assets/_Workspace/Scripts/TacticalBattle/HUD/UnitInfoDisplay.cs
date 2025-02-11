using System;
using _Workspace.Scripts.Data.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Workspace.Scripts.TacticalBattle.HUD
{
    [Serializable]
    public class ActionPointsBar
    {
        [SerializeField] private Image _mover;
        [SerializeField] private TextMeshProUGUI _pointsDisplay;

        public void SetValue(float current, float max)
        {
            _pointsDisplay.text = $"{current}/{max}";
            _mover.fillAmount = current / max;
        }
    }
    [Serializable]
    public class HitPointsBar
    {
        [SerializeField] private Image _mover;
        [SerializeField] private TextMeshProUGUI _pointsDisplay;

        public void SetValue(float current, float max)
        {
            _pointsDisplay.text = $"{current}/{max}";
            _mover.fillAmount = current / max;
        }
    }
    public class UnitInfoDisplay : MonoBehaviour
    {
        [SerializeField] private UnitSpawner _unitSpawner;
        [SerializeField] private TacticalBattlePlayerController _playerController;
        [SerializeField] private ActionPointsBar _actionPointsBar;
        [SerializeField] private HitPointsBar _hitPointsBar;

        private IUnitStats _selectedUnitStats;
        private void Awake()
        {
            _unitSpawner.OnSpawnEnd += SpawnEnd;
            _playerController.OnSelectUnit += UnitSelect;

            ResetDisplay();
        }

        private void ResetDisplay()
        {
            _hitPointsBar.SetValue(0, 1);
            _actionPointsBar.SetValue(0, 1);
        }

        private void OnDestroy()
        {
            _unitSpawner.OnSpawnEnd -= SpawnEnd;
            _playerController.OnSelectUnit -= UnitSelect;
        }

        private void UnitSelect(IUnitStats stats)
        {
            if (_selectedUnitStats != null)
            {
                _selectedUnitStats.CurrentActionPoints.Changed -= UpdateActionPointsDisplay;
                _selectedUnitStats.CurrentHitPoints.Changed -= UpdateHitPointsDisplay;
            }

            _selectedUnitStats = stats;
            _selectedUnitStats.CurrentActionPoints.Changed += UpdateActionPointsDisplay;
            _selectedUnitStats.CurrentHitPoints.Changed += UpdateHitPointsDisplay;
            
            UpdateActionPointsDisplay(_selectedUnitStats.CurrentActionPoints.value);
            UpdateHitPointsDisplay(_selectedUnitStats.CurrentHitPoints.value);
        }

        private void UpdateActionPointsDisplay(float current)
        {
            var maxActionPoints = _selectedUnitStats.MaxActionPoints;
            _actionPointsBar.SetValue(current, maxActionPoints);
        }
        
        private void UpdateHitPointsDisplay(float current)
        {
            var maxHitPoints = _selectedUnitStats.MaxHitPoints;
            _hitPointsBar.SetValue(current, maxHitPoints);
        }

        private void SpawnEnd()
        {
            
        }
    }
}