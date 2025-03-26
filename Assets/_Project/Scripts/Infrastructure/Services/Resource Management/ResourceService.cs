using _Project.Scripts.Infrastructure.Observable;
using _Project.Scripts.Infrastructure.Services.PersistentProgress;

namespace _Project.Scripts.Infrastructure.Services.Resource_Management
{
    public abstract class ResourceService<T> : IService
    {
        public abstract IReadonlyObservableVariable<T> ObservableValue { get; }

        protected IPersistentProgressService PersistentProgressService { get; }
        protected SaveLoadService SaveLoadService { get; }

        protected ResourceService(IPersistentProgressService persistentProgressService,
            SaveLoadService saveLoadService)
        {
            PersistentProgressService = persistentProgressService;
            SaveLoadService = saveLoadService;
        }

        public abstract bool Add(object sender, int amount);

        public abstract bool Spend(object sender, int amount);
    }
}