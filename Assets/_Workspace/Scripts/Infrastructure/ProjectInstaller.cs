using _Workspace.Scripts.SaveLoad;
using Zenject;

namespace Infrastructure
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<SaveLoaderBattleUnits>().AsSingle();
        }
    }
}