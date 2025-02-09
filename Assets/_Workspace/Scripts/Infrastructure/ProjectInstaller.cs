using _Workspace.Scripts.SaveLoad;
using Extra;
using Zenject;

namespace _Workspace.Scripts.Infrastructure
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<SaveLoaderBattleUnits>().AsSingle();
            Container.Bind<MathOperations>().AsSingle();
        }
    }
}