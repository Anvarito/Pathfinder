using System;
using _Workspace.Scripts.PlayerInput;
using _Workspace.Scripts.TacticalBattle;
using UnityEngine;
using Zenject;

namespace _Workspace.Scripts.GameCamera
{
    public class SceneViewCamera : MonoBehaviour
    {
        [SerializeField] private Transform _camera;

        [Header("Movement Settings")] [SerializeField]
        private float _cameraTopBorder = 1;

        [SerializeField] private float _cameraDownBorder = -1;

        [SerializeField] private float _oneFloorStep = 3;

        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 200f; // Увеличено для работы с Input.GetAxis
        [SerializeField] private float panSpeed = 20f;

        private IInputService _inputService;
        private IFloorSwitchHandler _floorSwitchHandler;
        private Vector3 _targetCameraZoom;
        private float _targetPivotLift;
        private int _currentFloorLevel;
        private bool _runCameraZoom;
        private bool _runPivotLift;
        public event Action OnRotation;

        [Inject]
        public void Construct(IInputService inputService, IFloorSwitchHandler floorSwitchHandler)
        {
            _inputService = inputService;
            _floorSwitchHandler = floorSwitchHandler;

            _inputService.OnRightMousePress += HandleRotation;
            _inputService.OnMiddleMousePress += HandlePanning;
            _inputService.OnWheelAxis += HandleZoom;

            _inputService.OnMovePress += HorizontalMove;

            _floorSwitchHandler.CurrentLevel.Changed += LevelChange;
        }

        private void LevelChange(int newLevel)
        {
            _targetPivotLift = newLevel * 3;
            _runPivotLift = true;
        }

        private void OnDestroy()
        {
            _inputService.OnRightMousePress -= HandleRotation;
            _inputService.OnMiddleMousePress -= HandlePanning;
            _inputService.OnWheelAxis -= HandleZoom;

            _inputService.OnMovePress -= HorizontalMove;
        }


        private void CameraDownMove()
        {
            _targetCameraZoom += Vector3.down;
        }

        private void CameraUpMove()
        {
            _targetCameraZoom += Vector3.up;
        }

        private void HandleZoom(float amount)
        {
            _targetCameraZoom = _camera.transform.localPosition;
            _runCameraZoom = true;
            if (amount > 0)
                CameraUpMove();
            else if (amount < 0)
                CameraDownMove();
        }

        private void Update()
        {
            if (_runCameraZoom)
            {
                _targetCameraZoom.y =
                    Mathf.Clamp(_targetCameraZoom.y, _cameraDownBorder, _cameraTopBorder);
                _camera.transform.localPosition =
                    Vector3.Lerp(_camera.transform.localPosition, _targetCameraZoom, 0.1f);

                if (Mathf.Approximately(_camera.transform.localPosition.y, _targetCameraZoom.y))
                {
                    _runCameraZoom = false;
                }
            }

            if (_runPivotLift)
            {
                var position = transform.position;
                var newY = Mathf.Lerp(position.y, _targetPivotLift, 0.1f);
                var newLiftPosition = new Vector3(position.x, newY, position.z);
                transform.position = newLiftPosition;

                if (Mathf.Approximately(transform.position.y, _targetPivotLift))
                {
                    _runPivotLift = false;
                }
            }
        }

        private void HandleRotation()
        {
            float rotationAmountY = _inputService.MouseAxisMove.x * rotationSpeed * Time.deltaTime;
            transform.Rotate(0f, rotationAmountY, 0f, Space.World);

            float rotationAmountX = _inputService.MouseAxisMove.y * rotationSpeed * Time.deltaTime;
            _camera.transform.Rotate(-rotationAmountX, 0f, 0f, Space.Self);

            Vector3 cameraEuler = _camera.transform.localEulerAngles;
            if (cameraEuler.x > 180f)
                cameraEuler.x -= 360f;
            cameraEuler.x = Mathf.Clamp(cameraEuler.x, -90f, 90f);

            _camera.transform.localEulerAngles = cameraEuler;
        }


        private void HandlePanning()
        {
            Vector3 movement = transform.right * (-_inputService.MouseAxisMove.x) +
                               transform.forward * (-_inputService.MouseAxisMove.y);
            transform.position += movement * panSpeed * Time.deltaTime;
        }

        private void HorizontalMove(float x, float y)
        {
            Vector3 movement = Vector3.zero;
            Vector3 right = transform.right * x;
            Vector3 forward = transform.forward * y;

            forward.y = 0;
            forward.Normalize();

            movement += right + forward;

            transform.position += movement * Time.deltaTime * moveSpeed;
        }
    }
}