using System;
using _Workspace.Scripts.PlayerInput;
using Infrastructure.Extras;
using Zenject;

namespace _Workspace.Scripts.TacticalBattle
{
    public interface IFloorSwitchHandler
    {
        public ReactiveProperty<int> CurrentLevel { get; }
    }
    
    public class FloorSwitchHandler : IFloorSwitchHandler, IInitializable, IDisposable
    {
        private readonly IInputService _inputService;
        public ReactiveProperty<int> CurrentLevel { get; private set; } = new ReactiveProperty<int>(1);

        public FloorSwitchHandler(IInputService inputService)
        {
            _inputService = inputService;
        }

        public void Initialize()
        {
            _inputService.OnUpDirectionDown += LiftToUpLevel;
            _inputService.OnDownDirectionDown += LiftToDownLevel;
        }

        private void LiftToDownLevel()
        {
            CurrentLevel.value--;
        }

        private void LiftToUpLevel()
        {
            CurrentLevel.value++;
        }

        public void Dispose()
        {
            _inputService.OnUpDirectionDown -= LiftToUpLevel;
            _inputService.OnDownDirectionDown -= LiftToDownLevel;
        }
    }
}