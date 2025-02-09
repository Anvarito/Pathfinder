
using Infrastructure.Extras;
using UnityEngine;

namespace Data.Scripts
{
    public interface ICurrentActionPoints
    {
        public float MaxActionPoints { get; }
        public ReactiveProperty<float> CurrentActionPoints { get; }
    }
    public class UnitStats : ICurrentActionPoints
    {
        public float MaxActionPoints { get; private set; } = 40;
        public float MaxHitPoints { get; private set; } = 100;

        public ReactiveProperty<float> CurrentActionPoints { get; private set; } = new ReactiveProperty<float>(40);

        public UnitStats(float maxActionPoints, float maxHitPoints)
        {
            MaxActionPoints = maxActionPoints;
            MaxHitPoints = maxHitPoints;
            CurrentActionPoints = new ReactiveProperty<float>(maxActionPoints);
        }

        public void DecreaseActionPoints(float amount)
        {
            CurrentActionPoints.value -= amount;
        }
    }

    public class UnitSaveData
    {
        public UnitStats UnitStats;
        public Vector3 Position;
        public Vector3 Rotation;

        public UnitSaveData(UnitStats unitStats, Vector3 position, Vector3 rotation)
        {
            UnitStats = unitStats;
            Position = position;
            Rotation = rotation;
        }
    }
}