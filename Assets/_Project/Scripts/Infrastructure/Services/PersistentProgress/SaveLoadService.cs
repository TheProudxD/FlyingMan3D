using _Project.Scripts.Data;
using YG;

namespace _Project.Scripts.Infrastructure.Services.PersistentProgress
{
    public class SaveLoadService : IService
    {
        private readonly IPersistentProgressService _progressService;
        
        private PlayerProgress _playerProgress;

        public SaveLoadService(IPersistentProgressService progressService) => _progressService = progressService;

        public void InformAll() { }

        public void Save()
        {
            YG2.saves.hints = _playerProgress.HintsNumber.Value;
            YG2.saves.level = _playerProgress.CurrentLevel.Value;
            YG2.SaveProgress();
        }

        public PlayerProgress LoadProgress()
        {
            _playerProgress = new PlayerProgress(YG2.saves.hints, YG2.saves.level);

            return _playerProgress;
        }
        
        public override string ToString() => _progressService.ToString();
    }
}