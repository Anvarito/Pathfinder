using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Workspace.Scripts.TacticalBattle
{
    public class TacticalBattlePlayerController : MonoBehaviour
    {
        [SerializeField] private Unit _hoveredUnit;
        [SerializeField] private Unit _selectedUnit;
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private GameObject _cursorPrefab;

        private Transform _cursor;
        private PathNode _targetPathNode;
        public event Action<Unit> OnSelectUnit;

        private void Awake()
        {
            _cursor = Instantiate(_cursorPrefab, transform).transform;
        }

        private void Update()
        {
            if(EventSystem.current.IsPointerOverGameObject())
            {
                HideCursor();
                return;
            }
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _groundMask))
            {
                if (hit.transform.TryGetComponent(out PathNode pathNode))
                {
                    HoverNode(pathNode);
                }
                
            }else
            {
                HideCursor();
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (_hoveredUnit)
                {
                    SelectedUnit();
                    return;
                }
            }

            if (Input.GetMouseButtonUp(1))
            {
                if (_selectedUnit)
                {
                    if (_selectedUnit.IsOnAction)
                    {
                        _selectedUnit.StopMoving();
                    }
                    else if(_targetPathNode)
                        _selectedUnit.LookAtNode(_targetPathNode);

                    return;
                }
            }

            if (_selectedUnit && !_hoveredUnit)
            {
                if (!_selectedUnit.IsOnAction)
                {
                    _selectedUnit.SearchPath(_targetPathNode);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (_selectedUnit)
                {
                    if (_selectedUnit.IsOnAction)
                    {
                        _selectedUnit.StopMoving();
                    }
                    else
                        _selectedUnit.ApproveMove();
                }
            }
        }

        private void HideCursor()
        {
            _cursor.gameObject.SetActive(false);
            _targetPathNode = null;
        }

        private void SelectedUnit()
        {
            if (_selectedUnit != null && _selectedUnit != _hoveredUnit)
            {
                _selectedUnit.Selected(false);
            }

            _selectedUnit = _hoveredUnit;
            _selectedUnit.Selected(true);
            OnSelectUnit?.Invoke(_selectedUnit);
        }

        private void HoverNode(PathNode pathNode)
        {
            _targetPathNode = pathNode;
            _cursor.gameObject.SetActive(true);
            _cursor.position = _targetPathNode.GridPosition;
            if (_targetPathNode.IsOcupied)
            {
                if (_selectedUnit != null)
                {
                    _selectedUnit.HidePath();
                }

                _hoveredUnit = _targetPathNode.UnitCurrent;
                _hoveredUnit.HoverHighlight();
            }
            else
                _hoveredUnit = null;
        }
    }
}