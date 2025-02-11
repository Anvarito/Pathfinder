using _Workspace.Scripts.TacticalBattle;
using Infrastructure.Extras;

namespace _Workspace.Scripts.Data.Scripts
{
    public interface IUnitStats
    {
        public float MaxHitPoints { get; }
        public ReactiveProperty<float> CurrentHitPoints { get; }
   
        public float MaxActionPoints { get; }
        public ReactiveProperty<float> CurrentActionPoints { get; }
    }

    public class UnitStats : IUnitStats
    {
        public float MaxHitPoints { get; private set; } = 100;
        public ReactiveProperty<float> CurrentHitPoints { get; }

        public float MaxActionPoints { get; private set; } = 40;
        public ReactiveProperty<float> CurrentActionPoints { get; private set; } = new ReactiveProperty<float>(40);

        public UnitStats(UnitStatsSaveData saveData)
        {
            MaxActionPoints = saveData.MaxActionPoints;
            MaxHitPoints = saveData.MaxHitPoints;
            CurrentActionPoints = new ReactiveProperty<float>(saveData.CurrentActionPoints);
            CurrentHitPoints = new ReactiveProperty<float>(saveData.CurrentHitPoints);
        }

        public UnitStats()
        {
            MaxActionPoints = 40;
            MaxHitPoints = 100;
            CurrentActionPoints = new ReactiveProperty<float>(MaxActionPoints);
            CurrentHitPoints = new ReactiveProperty<float>(MaxHitPoints);
        }

        public void DecreaseActionPoints(float amount)
        {
            CurrentActionPoints.value -= amount;
        }
        
        public void DecreaseHitPoints(float amount)
        {
            CurrentHitPoints.value -= amount;
        }

        public void RestoreActionPoints()
        {
            CurrentActionPoints.value = MaxActionPoints;
        }

        public UnitStatsSaveData GetSaveData()
        {
            UnitStatsSaveData statsSaveData = new UnitStatsSaveData()
            {
                CurrentActionPoints = CurrentActionPoints.value,
                MaxActionPoints = MaxActionPoints,
                MaxHitPoints = MaxHitPoints,
                CurrentHitPoints = CurrentHitPoints.value
            };
            return statsSaveData;
        }
    }

    public class UnitStatsSaveData
    {
        public float CurrentActionPoints = 40;
        public float MaxActionPoints = 40;
        public float MaxHitPoints = 100;
        public float CurrentHitPoints = 100;
    }

    public class UnitSaveData
    {
        public string UnitName = "VASIA";
        public Unit.UnitTransformSaveData UnitTransformSaveData;
        public UnitStatsSaveData UnitStatsSaveData;

        public UnitSaveData(string unitName, Unit.UnitTransformSaveData transformSaveData,
            UnitStatsSaveData statsSaveData)
        {
            UnitName = unitName;
            UnitTransformSaveData = transformSaveData;
            UnitStatsSaveData = statsSaveData;
        }
    }
}