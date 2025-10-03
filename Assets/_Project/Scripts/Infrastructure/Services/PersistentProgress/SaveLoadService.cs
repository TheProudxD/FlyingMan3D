using _Project.Scripts.Data;
using UnityEngine;
using YG;

namespace _Project.Scripts.Infrastructure.Services.PersistentProgress
{
    public class SaveLoadService : IService
    {
        private readonly IPersistentProgressService _progressService;

        private PlayerProgress _playerProgress;
        private PowerupProgress _powerupProgress;

        public SaveLoadService(IPersistentProgressService progressService) => _progressService = progressService;

        public void InformAll() { }

        public void Save()
        {
            PlayerPrefs.SetInt("CurrentLevel", _playerProgress.CurrentLevel.Value);
            PlayerPrefs.SetInt("Money", _playerProgress.MoneyNumber.Value);
            PlayerPrefs.SetInt("RichestLevel", _playerProgress.RichestLevel.Value);
            
            PlayerPrefs.SetInt("healthPowerup", _powerupProgress.health);
            PlayerPrefs.SetInt("healthProgressPowerup", _powerupProgress.healthProgress);
            PlayerPrefs.SetFloat("movingSpeedPowerup", _powerupProgress.movingSpeed);
            PlayerPrefs.SetInt("movingSpeedProgressPowerup", _powerupProgress.movingSpeedProgress);
            PlayerPrefs.SetFloat("flyingControlPowerup", _powerupProgress.flyingControl);
            PlayerPrefs.SetInt("flyingControlProgressPowerup", _powerupProgress.flyingControlProgress);
            
            PlayerPrefs.Save();
        }

        public (PlayerProgress, PowerupProgress) LoadProgress()
        {
            _playerProgress = new PlayerProgress(PlayerPrefs.GetInt("CurrentLevel", 1),
                PlayerPrefs.GetInt("Money", 250), PlayerPrefs.GetInt("RichestLevel", -1));

            _powerupProgress = new PowerupProgress(PlayerPrefs.GetInt("healthPowerup", 1),
                PlayerPrefs.GetInt("healthProgressPowerup", 1),
                PlayerPrefs.GetFloat("movingSpeedPowerup", 3.5f),
                PlayerPrefs.GetInt("movingSpeedProgressPowerup", 1),
                PlayerPrefs.GetFloat("flyingControlPowerup", 65f),
                PlayerPrefs.GetInt("flyingControlProgressPowerup", 1));

            return (_playerProgress, _powerupProgress);
        }

        public override string ToString() => _progressService.ToString();
    }
}