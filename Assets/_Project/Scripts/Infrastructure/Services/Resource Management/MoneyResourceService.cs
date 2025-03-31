using _Project.Scripts.Infrastructure.Observable;
using _Project.Scripts.Infrastructure.Services.PersistentProgress;
using _Project.Scripts.Infrastructure.Services.Resource_Management;

namespace _Project.Scripts.Infrastructure.Services.Resources
{
    public class MoneyResourceService : ResourceService<int>
    {
        public MoneyResourceService(IPersistentProgressService persistentProgressService,
            SaveLoadService saveLoadService) : base(persistentProgressService, saveLoadService) { }

        public override IReadonlyObservableVariable<int> ObservableValue => PersistentProgressService.Progress.MoneyNumber;
        
        public bool IsEnough(int amount) => PersistentProgressService.Progress.MoneyNumber.Value >= amount;

        public override bool Add(object sender, int amount)
        {
            if (amount < 0)
            {
                UnityEngine.Debug.LogError("Value cannot be negative");
                return false;
            }

            PersistentProgressService.Progress.MoneyNumber.Value += amount;
            // SaveLoadService.SetTotalMoneyEarned(amount);
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

            if (PersistentProgressService.Progress.MoneyNumber.Value - amount < 0)
            {
                UnityEngine.Debug.LogError("You can't spend more than the current amount");
                return false;
            }

            PersistentProgressService.Progress.MoneyNumber.Value -= amount;
            SaveLoadService.Save();
            return true;
        }

        /*
        private readonly bool _initialized;

        private void CheckInit()
        {
            if (_initialized == false)
                throw new Exception("Initialize method wasn't called!");
        }*/
    }
}