
using Infrastructure.Extras;
using UnityEngine;

namespace Data.Scripts
{
    public interface IHitPoints
    {
        public float MaxHitPoints { get; }
        public ReactiveProperty<float> CurrentHitPoints { get; }
    }
    public interface IActionPoints
    {
        public float MaxActionPoints { get; }
        public ReactiveProperty<float> CurrentActionPoints { get; }
    }
    public class UnitStats : IActionPoints, IHitPoints
    {
        public float MaxHitPoints { get; private set; } = 100;
        public ReactiveProperty<float> CurrentHitPoints { get; }

        public float MaxActionPoints { get; private set; } = 40;
        public ReactiveProperty<float> CurrentActionPoints { get; private set; } = new ReactiveProperty<float>(40);

        public UnitStats(float maxActionPoints, float maxHitPoints, float currentActionPoints, float currentHitPoints)
        {
            MaxActionPoints = maxActionPoints;
            MaxHitPoints = maxHitPoints;
            CurrentActionPoints = new ReactiveProperty<float>(currentActionPoints);
            CurrentHitPoints = new ReactiveProperty<float>(currentHitPoints);
        }

        public void DecreaseActionPoints(float amount)
        {
            CurrentActionPoints.value -= amount;
        }
    }

    public class UnitSaveData
    {
        public string UnitName = "VASIA";
        public Vector3 Position = Vector3.zero;
        public Vector3 Rotation = Vector3.zero;
        public float CurrentActionPoints = 40;
        public float MaxActionPoints = 40;
        public float MaxHitPoints = 100;
        public float CurrentHitPoints = 100;
        public Vector3 CurrentNode;
        public UnitSaveData(string unitName, Vector3 currentNode, Vector3 position, Vector3 rotation, float currentActionPoints, float maxActionPoints,float maxHitPoints, float currentHitPoint)
        {
            UnitName = unitName;
            CurrentNode = currentNode;
            Position = position;
            Rotation = rotation;
            CurrentActionPoints = currentActionPoints;
            MaxHitPoints = maxHitPoints;
            MaxActionPoints = maxActionPoints;
            CurrentHitPoints = currentHitPoint;
        }
    }
}