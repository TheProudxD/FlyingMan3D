using _Project.Scripts.Tools;
using Reflex.Core;

namespace _Project.Scripts.Infrastructure.Services.AssetManagement
{
    public class ProjectObjectInjector : IService
    {
        private Container _container;

        public void SetContainer(Container container) => _container = container;

        public void Inject<T>(T target)
        {
            if (_container == null || target == null)
                return;

            _container.Inject(target);
        }
    }
}
