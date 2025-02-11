using System;
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
    public class UnitInfoDisplay : MonoBehaviour
    {
        [SerializeField] private UnitSpawner _unitSpawner;
        [SerializeField] private TacticalBattlePlayerController _playerController;
        [SerializeField] private ActionPointsBar _actionPointsBar;

        private Unit _selectedUnit;
        private void Awake()
        {
            _unitSpawner.OnSpawnEnd += SpawnEnd;
            _playerController.OnSelectUnit += UnitSelect;
        }

        private void OnDestroy()
        {
            _unitSpawner.OnSpawnEnd -= SpawnEnd;
            _playerController.OnSelectUnit -= UnitSelect;
        }

        private void UnitSelect(Unit unit)
        {
            if (_selectedUnit != null)
                _selectedUnit.Stats.CurrentActionPoints.Changed -= UpdateActionPointsDisplay;

            _selectedUnit = unit;
            _selectedUnit.Stats.CurrentActionPoints.Changed += UpdateActionPointsDisplay;
            UpdateActionPointsDisplay(_selectedUnit.Stats.CurrentActionPoints.value);
        }

        private void UpdateActionPointsDisplay(float current)
        {
            var maxActionPoints = _selectedUnit.Stats.MaxActionPoints;
            _actionPointsBar.SetValue(current, maxActionPoints);
        }

        private void SpawnEnd()
        {
            
        }
    }
}