using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Workspace.Scripts.TacticalBattle.HUD
{
    public class MainButtonListener : MonoBehaviour
    {
        [SerializeField] private Button _endTurnButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _loadButton;
        [SerializeField] private Button _clearButton;
        [SerializeField] private Button _spawnOneButton;

        public event Action OnTurnEndPress;
        public event Action OnSpawnOnePress;
        public event Action OnLoadPress;
        public event Action OnSavePress;
        public event Action OnClearPress;

        private void Awake()
        {
            _endTurnButton.onClick.AddListener(EndTurnClick);
            _saveButton.onClick.AddListener(SaveClick);
            _loadButton.onClick.AddListener(LoadClick);
            _clearButton.onClick.AddListener(ClearClick);
            _spawnOneButton.onClick.AddListener(SpawnOneClick);
        }

        private void OnDestroy()
        {
            _endTurnButton.onClick.RemoveListener(EndTurnClick);
            _saveButton.onClick.RemoveListener(SaveClick);
            _loadButton.onClick.RemoveListener(LoadClick);
            _clearButton.onClick.RemoveListener(ClearClick);
            _spawnOneButton.onClick.RemoveListener(SpawnOneClick);
        }

        private void SpawnOneClick()
        {
            OnSpawnOnePress?.Invoke();
        }

        private void ClearClick()
        {
            OnClearPress?.Invoke();
        }

        private void LoadClick()
        {
            OnLoadPress?.Invoke();
        }

        private void SaveClick()
        {
            OnSavePress?.Invoke();
        }

        private void EndTurnClick()
        {
            OnTurnEndPress?.Invoke();
        }
    }
}