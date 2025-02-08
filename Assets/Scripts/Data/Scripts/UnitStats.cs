
using Infrastructure.Extras;

namespace Data.Scripts
{
    public interface ICurrentActionPoints
    {
        public float MaxActionPoints { get; }
        public ReactiveProperty<float> CurrentActionPoints { get; }
    }
    public class UnitStats : ICurrentActionPoints
    {
        public float MaxActionPoints { get; private set; }
        public float MaxHitPoints { get; private set; }
        
        public ReactiveProperty<float> CurrentActionPoints { get; private set; }

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
}