using System;
using UnityEngine;

namespace TacticalBattle
{
    public class UnitRotator : MonoBehaviour
    {
        private int _nextRotationStep;  // Сохранение шага поворота (на 45 градусов)
        private float _lerpTime = 0f;  // Время для Lerp
        private float _lerpDuration = 0.2f;  // Длительность поворота
        private float _targetRotationY;  // Цель поворота по оси Y
        private bool _isRotating;
        public event Action OnStepEnded;
        public event Action OnRotateStart; 

        
        public void RotateToNext(PathNode nextNode)
        {
            CalculateToNear(nextNode);
            
        }

        private void CalculateToNear(PathNode nextNode)
        {
            Vector3 targetDirection = nextNode.GridPosition - transform.position;
            int targetRotation = Mathf.RoundToInt(Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg);
            int currentRotation = Mathf.RoundToInt(Mathf.DeltaAngle(0, transform.eulerAngles.y));
            int angleDiff = (int)Mathf.DeltaAngle(currentRotation, targetRotation);

            if (Mathf.Abs(angleDiff) < 45)
            {
                _nextRotationStep = 0;  // Не требуется поворот
                return;
            }
            
            _nextRotationStep = (int)Mathf.Sign(angleDiff) * 45;
            _targetRotationY = (currentRotation + _nextRotationStep) % 360;
            _lerpTime = 0f;
            _isRotating = true;
            OnRotateStart?.Invoke();
        }

        private void Update()
        {
            if (_isRotating)
            {
                _lerpTime += Time.deltaTime;
                float alpha = _lerpTime / _lerpDuration;
                float currentRotationY = Mathf.LerpAngle(transform.eulerAngles.y, _targetRotationY, alpha);
                transform.eulerAngles = new Vector3(0, currentRotationY, 0);
                if (alpha >= 1)
                {
                    transform.eulerAngles = new Vector3(0, _targetRotationY, 0); 
                    _isRotating = false;
                    OnStepEnded?.Invoke();
                }
            }
        }

        
    }
}