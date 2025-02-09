using _Workspace.Scripts.TacticalBattle;
using _Workspace.Scripts.TacticalBattle.HUD;
using _Workspace.Scripts.TacticalBattle.PathFInding;
using UnityEngine;
using Zenject;

namespace _Workspace.Scripts.Infrastructure
{
    public class BattleSceneInstaller : MonoInstaller
    {
        [SerializeField] private UnitSpawner _unitSpawner;
        [SerializeField] private MainButtonListener _mainButtonListener;
        [SerializeField] private GridManager _gridManager;
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<TacticalBattleTurnController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<UnitSpawner>().FromInstance(_unitSpawner).AsSingle();
            Container.Bind<MainButtonListener>().FromInstance(_mainButtonListener).AsSingle();
            Container.Bind<GridManager>().FromInstance(_gridManager).AsSingle().NonLazy();
            Container.BindInterfacesTo<AstarPathFinder>().AsSingle();
        }
    }
}