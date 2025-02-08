using System;
using Extra;
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
        
        public void LookAtNode(PathNode targetPathNode)
        {
            Vector3 targetDirection = targetPathNode.GridPosition - transform.position;
            int targetRotation = Mathf.RoundToInt(Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg);
            int currentRotation = Mathf.RoundToInt(Mathf.DeltaAngle(0, transform.eulerAngles.y));
            int angleDiff = (int)Mathf.DeltaAngle(currentRotation, targetRotation);

            int rotationCount = Mathf.Abs(angleDiff) / 45;

            for (int i = 0; i < rotationCount; i++)
            {
                int rotationAmount = (int)Mathf.Sign(angleDiff) * 45;
                transform.Rotate(0, rotationAmount, 0);
                currentRotation = (currentRotation + rotationAmount) % 360;
                angleDiff = (int)Mathf.DeltaAngle(currentRotation, targetRotation);
            }
        }
        
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

            // Если угол меньше 45 градусов, нет необходимости поворачивать
            if (Mathf.Abs(angleDiff) < 45)
            {
                _nextRotationStep = 0;  // Не требуется поворот
                return;
            }

            
            // Рассчитываем следующий шаг поворота
            _nextRotationStep = (int)Mathf.Sign(angleDiff) * 45;

            // Определяем конечный угол поворота
            _targetRotationY = (currentRotation + _nextRotationStep) % 360;

            // Сбрасываем время для плавного поворота
            _lerpTime = 0f;
            _isRotating = true;  // Начинаем процесс поворота
        }

        private void Update()
        {
            if (_isRotating)
            {
                // Плавно вращаем с использованием Lerp
                _lerpTime += Time.deltaTime / _lerpDuration;

                // Текущий угол поворота с плавным интерполированием
                float currentRotationY = Mathf.LerpAngle(transform.eulerAngles.y, _targetRotationY, _lerpTime);
                transform.eulerAngles = new Vector3(0, currentRotationY, 0);

                // Если поворот завершён
                if (_lerpTime >= 1)
                {
                    transform.eulerAngles = new Vector3(0, _targetRotationY, 0);  // Точно устанавливаем кратное 45 значение
                    _isRotating = false;
                    OnStepEnded?.Invoke();  // Оповещаем о завершении шага
                }
            }
        }

        
    }
}