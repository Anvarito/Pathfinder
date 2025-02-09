using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Workspace.Scripts.TacticalBattle.HUD
{
    public class MainButtonListener : MonoBehaviour
    {
        [SerializeField] private Button _endTurnButton;

        public event Action OnTurnEndPress;

        private void Awake()
        {
            _endTurnButton.onClick.AddListener(EndTurnClick);
        }

        private void OnDestroy()
        {
            _endTurnButton.onClick.RemoveListener(EndTurnClick);
        }

        private void EndTurnClick()
        {
            OnTurnEndPress?.Invoke();
        }
    }
}