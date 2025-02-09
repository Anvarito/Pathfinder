using _Workspace.Scripts.TacticalBattle;
using _Workspace.Scripts.TacticalBattle.HUD;
using UnityEngine;
using Zenject;

namespace Infrastructure
{
    public class BattleSceneInstaller : MonoInstaller
    {
        [SerializeField] private UnitSpawner _unitSpawner;
        [SerializeField] private MainButtonListener _mainButtonListener;
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<TacticalBattleTurnController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<UnitSpawner>().FromInstance(_unitSpawner).AsSingle();
            Container.Bind<MainButtonListener>().FromInstance(_mainButtonListener).AsSingle();
        }
    }
}