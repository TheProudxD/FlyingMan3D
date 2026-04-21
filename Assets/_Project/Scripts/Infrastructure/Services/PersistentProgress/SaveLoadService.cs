using _Project.Scripts.Data;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.PersistentProgress
{
    public class SaveLoadService : IService
    {
        private readonly IPersistentProgressService _progressService;

        public SaveLoadService(IPersistentProgressService progressService) => _progressService = progressService;

        public void Save()
        {
            EnsureProgressInitialized();

            PlayerProgress playerProgress = _progressService.Progress;
            PowerupProgress powerupProgress = _progressService.PowerupProgress;

            PlayerPrefs.SetInt("CurrentLevel", playerProgress.CurrentLevel.Value);
            PlayerPrefs.SetInt("Money", playerProgress.MoneyNumber.Value);
            PlayerPrefs.SetInt("RichestLevel", playerProgress.RichestLevel.Value);
            
            PlayerPrefs.SetInt("healthPowerup", powerupProgress.health);
            PlayerPrefs.SetInt("healthProgressPowerup", powerupProgress.healthProgress);
            PlayerPrefs.SetFloat("movingSpeedPowerup", powerupProgress.movingSpeed);
            PlayerPrefs.SetInt("movingSpeedProgressPowerup", powerupProgress.movingSpeedProgress);
            PlayerPrefs.SetFloat("flyingControlPowerup", powerupProgress.flyingControl);
            PlayerPrefs.SetInt("flyingControlProgressPowerup", powerupProgress.flyingControlProgress);
            
            PlayerPrefs.Save();
        }

        public void LoadProgress()
        {
            _progressService.Progress = new PlayerProgress(PlayerPrefs.GetInt("CurrentLevel", 1),
                PlayerPrefs.GetInt("Money", 250), PlayerPrefs.GetInt("RichestLevel", -1));

            _progressService.PowerupProgress = new PowerupProgress(PlayerPrefs.GetInt("healthPowerup", 1),
                PlayerPrefs.GetInt("healthProgressPowerup", 1),
                PlayerPrefs.GetFloat("movingSpeedPowerup", 3.5f),
                PlayerPrefs.GetInt("movingSpeedProgressPowerup", 1),
                PlayerPrefs.GetFloat("flyingControlPowerup", 65f),
                PlayerPrefs.GetInt("flyingControlProgressPowerup", 1));
        }

        private void EnsureProgressInitialized()
        {
            _progressService.Progress ??= new PlayerProgress(1, 250, -1);
            _progressService.PowerupProgress ??= new PowerupProgress(1, 1, 3.5f, 1, 65f, 1);
        }

        public override string ToString() => _progressService.ToString();
    }
}
