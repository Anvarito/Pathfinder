using System;
using System.Collections.Generic;
using _Workspace.Scripts.Data.Scripts;
using _Workspace.Scripts.PlayerInput;
using _Workspace.Scripts.TacticalBattle.PathFInding;
using Extra;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace _Workspace.Scripts.TacticalBattle
{
    public class TacticalBattlePlayerController : MonoBehaviour
    {
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private GameObject _cursorPrefab;
        [SerializeField] private PathDrawer _pathDrawer;

        private Unit _hoveredUnit;
        private Unit _prevHoveredUnit;
        private Unit _selectedUnit;
        private Transform _cursor;
        private PathNode _targetPathNode;
        private IPathFinder _pathFinder;
        private List<PathNode> _path;
        private IInputService _inputService;
        public event Action<IUnitStats> OnSelectUnit;

        [Inject]
        public void Construct(IPathFinder pathFinder, IInputService inputService)
        {
            _pathFinder = pathFinder;
            _inputService = inputService;

            _inputService.OnLeftMouseUp += OnLeftMouseUp;
            _inputService.OnRightMouseUp += OnRightMouseUp;
        }

        private void OnDestroy()
        {
            _inputService.OnLeftMouseUp -= OnLeftMouseUp;
            _inputService.OnRightMouseUp -= OnRightMouseUp;
        }


        private void Awake()
        {
            _cursor = Instantiate(_cursorPrefab, transform).transform;
        }

        private void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject()) // above ui
            {
                CursorToggleActive();
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _groundMask))
            {
                if (hit.transform.TryGetComponent(out PathNode pathNode))
                {
                    HoverNode(pathNode);
                    HighlightUnit();
                }
            }
            else
            {
                _targetPathNode = null;
            }

            TryFindPathForSelectedUnit();

            CursorToggleActive();
        }


        private void OnRightMouseUp()
        {
            if (_selectedUnit)
            {
                if (_selectedUnit.IsOnAction)
                {
                    _selectedUnit.StopMoving();
                }
                else if (_targetPathNode)
                    _selectedUnit.LookAtNode(_targetPathNode);
            }
        }

        private void OnLeftMouseUp()
        {
            if (_hoveredUnit && _selectedUnit != _hoveredUnit)
            {
                SelectedUnit();
                return;
            }

            if (_selectedUnit)
            {
                if (_selectedUnit.IsOnAction)
                {
                    _selectedUnit.StopMoving();
                }
                else
                {
                    if (_path != null)
                        _selectedUnit.ApproveMove(_path);
                }
            }
        }

        private void TryFindPathForSelectedUnit()
        {
            bool unitSelectedAndStay = _selectedUnit && !_selectedUnit.IsOnAction;
            bool unitHaveMoveCost =
                _selectedUnit && _selectedUnit.Stats.CurrentActionPoints.value >= Constants.STEP_COST;

            if (
                _targetPathNode
                && !_targetPathNode.IsOcupied
                && unitSelectedAndStay
                && unitHaveMoveCost
            )
            {
                if (TryFindPath())
                {
                    _pathDrawer.UpdatePathVisualization(_path);
                }
                else
                {
                    HidePath();
                }
            }
            else
            {
                _path = null;
                HidePath();
            }
        }

        private bool TryFindPath()
        {
            _path = _pathFinder.FindPath(
                _selectedUnit.CurrentPathNode,
                _targetPathNode,
                _selectedUnit.Stats.CurrentActionPoints.value,
                _selectedUnit.transform.rotation.eulerAngles.y
            );

            return _path != null;
        }

        private void HidePath()
        {
            _pathDrawer.ClearPathVisualization();
        }

        private void CursorToggleActive()
        {
            _cursor.gameObject.SetActive(_targetPathNode);
        }

        private void SelectedUnit()
        {
            if (_selectedUnit != null)
            {
                _selectedUnit.UnitHighlighter.SetSelected(false);
            }

            _selectedUnit = _hoveredUnit;
            _selectedUnit.UnitHighlighter.SetSelected(true);
            OnSelectUnit?.Invoke(_selectedUnit.Stats);
        }

        private void HoverNode(PathNode pathNode)
        {
            _targetPathNode = pathNode;
            _cursor.position = _targetPathNode.GridPosition;
        }

        private void HighlightUnit()
        {
            // Если на текущей ноде нет юнита, убираем выделение
            if (!_targetPathNode.IsOcupied)
            {
                // Если был наведён юнит, снимаем с него выделение
                if (_hoveredUnit)
                    _hoveredUnit.UnitHighlighter.SetHighlight(false);
        
                // Обнуляем ссылки на текущий и предыдущий юнит
                _hoveredUnit = null;
                _prevHoveredUnit = null;
            }
            else
            {
                // Получаем текущего юнита на целевой ноде
                _hoveredUnit = _targetPathNode.UnitCurrent;

                // Если юнит не выбран, то выделяем его при наведении
                if (_hoveredUnit != _selectedUnit)
                {
                    if (_hoveredUnit != _prevHoveredUnit)
                    {
                        // Снимаем выделение с предыдущего юнита, если он был
                        _prevHoveredUnit?.UnitHighlighter.SetHighlight(false);
                
                        // Выделяем текущий юнит
                        _hoveredUnit.UnitHighlighter.SetHighlight(true);
                    }
                }
                else
                {
                    // Если юнит выбран, убираем выделение
                    _hoveredUnit.UnitHighlighter.SetHighlight(false);
                }

                // Обновляем ссылку на предыдущий юнит
                _prevHoveredUnit = _hoveredUnit;
            }
        }
    }
}