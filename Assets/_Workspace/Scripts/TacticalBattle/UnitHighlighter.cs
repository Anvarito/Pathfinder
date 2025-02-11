using System;
using UnityEngine;

namespace _Workspace.Scripts.TacticalBattle
{
    public class UnitHighlighter : MonoBehaviour
    {
        [SerializeField] private GameObject _marker;
        [SerializeField] private GameObject _markerSelected;

        private void Awake()
        {
            SetSelected(false);
            SetHighlight(false);
        }

        public void SetSelected(bool isSelected)
        {
            _markerSelected.SetActive(isSelected);
            _marker.SetActive(false);
        }

        public void SetHighlight(bool isHighlight)
        {
            _marker.SetActive(isHighlight);
        }
    }
}