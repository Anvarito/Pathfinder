using _Workspace.Scripts.PlayerInput;
using _Workspace.Scripts.TacticalBattle;
using _Workspace.Scripts.TacticalBattle.HUD;
using _Workspace.Scripts.TacticalBattle.PathFInding;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace _Workspace.Scripts.Infrastructure
{
    public class BattleSceneInstaller : MonoInstaller
    {
        [SerializeField] private UnitSpawner _unitSpawner;
        [SerializeField] private MainButtonListener _mainButtonListener;
        [FormerlySerializedAs("_gridManager")] [SerializeField] private GraphMaster graphMaster;
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<TacticalBattleTurnController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<UnitSpawner>().FromInstance(_unitSpawner).AsSingle();
            Container.Bind<MainButtonListener>().FromInstance(_mainButtonListener).AsSingle();
            Container.Bind<GraphMaster>().FromInstance(graphMaster).AsSingle().NonLazy();
            Container.BindInterfacesTo<AstarPathFinder>().AsSingle();
            Container.BindInterfacesTo<InputService>().AsSingle();
            Container.BindInterfacesTo<FloorSwitchHandler>().AsSingle();
        }
    }
}