using _Project.Scripts.Infrastructure.Observable;
using _Project.Scripts.Infrastructure.Services.PersistentProgress;
using _Project.Scripts.Infrastructure.Services.Resource_Management;

namespace _Project.Scripts.Infrastructure.Services.Resources
{
    public class LevelResourceService : ResourceService<int>
    {
        public LevelResourceService(IPersistentProgressService persistentProgressService,
            SaveLoadService saveLoadService) : base(persistentProgressService, saveLoadService) { }

        public override IReadonlyObservableVariable<int> ObservableValue =>
            PersistentProgressService.Progress.CurrentLevel;

        public ObservableVariable<int> Current =>
            _current ??= new ObservableVariable<int>(PersistentProgressService.Progress.CurrentLevel.Value);

        private ObservableVariable<int> _current;

        public override bool Add(object sender, int amount)
        {
            if (amount < 0)
            {
                UnityEngine.Debug.LogError("Value cannot be negative");
                return false;
            }

            if (Current.Value != ObservableValue.Value)
                return false;

            PersistentProgressService.Progress.CurrentLevel.Value += amount;
            SaveLoadService.Save();
            return true;
        }

        public override bool Spend(object sender, int amount)
        {
            if (amount < 0)
            {
                UnityEngine.Debug.LogError("Value cannot be negative");
                return false;
            }

            if (PersistentProgressService.Progress.CurrentLevel.Value < amount)
            {
                UnityEngine.Debug.LogError("You can't spend more than is in current amount");
                return false;
            }

            PersistentProgressService.Progress.CurrentLevel.Value -= amount;
            SaveLoadService.Save();
            return true;
        }

        public bool Increase(object sender) => Add(sender, 1);
    }
}