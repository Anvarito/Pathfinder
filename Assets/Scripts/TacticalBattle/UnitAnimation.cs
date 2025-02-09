using UnityEngine;

namespace TacticalBattle
{
    public class UnitAnimation : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private UnitRotator _rotator;
        [SerializeField] private UnitMover _unitMover;
        [SerializeField] private Unit _unit;
        private readonly int Run = Animator.StringToHash("Run");
        private readonly int Rotation = Animator.StringToHash("Rot");

        private void Awake()
        {
            _unitMover.OnMoveStart += Move;
            _rotator.OnRotateStart += Rotate;
            _unit.OnAllActionStop += FullStop;
        }

        private void OnDestroy()
        {
            _rotator.OnRotateStart -= Rotate;
            _unitMover.OnMoveStart -= Move;
            _unit.OnAllActionStop -= FullStop;
        }

        private void Rotate()
        {
            _animator.SetTrigger(Rotation);
        }

        private void FullStop()
        {
            _animator.SetBool(Run, false);
        }

        private void Move()
        {
            _animator.SetBool(Run, true);
        }
    }
}