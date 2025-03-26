using _Project.Scripts.Infrastructure.Observable;
using _Project.Scripts.Infrastructure.Services.PersistentProgress;
using _Project.Scripts.Infrastructure.Services.Resource_Management;

namespace _Project.Scripts.Infrastructure.Services.Resources
{
    public class HintResourceService : ResourceService<int>
    {
        public HintResourceService(IPersistentProgressService persistentProgressService,
            SaveLoadService saveLoadService) : base(persistentProgressService, saveLoadService) { }

        public override IReadonlyObservableVariable<int> ObservableValue =>
            PersistentProgressService.Progress.HintsNumber;

        public bool IsOutOfHints() => PersistentProgressService.Progress.HintsNumber.Value <= 0;

        public override bool Add(object sender, int amount)
        {
            if (amount < 0)
            {
                UnityEngine.Debug.LogError("Value cannot be negative");
                return false;
            }

            PersistentProgressService.Progress.HintsNumber.Value += amount;
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

            if (PersistentProgressService.Progress.HintsNumber.Value < amount)
            {
                UnityEngine.Debug.LogError("You can't spend more than is in current amount");
                return false;
            }

            PersistentProgressService.Progress.HintsNumber.Value -= amount;
            SaveLoadService.Save();
            return true;
        }
    }
}