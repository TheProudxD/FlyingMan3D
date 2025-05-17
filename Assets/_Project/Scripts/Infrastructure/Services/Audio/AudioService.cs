using System.Collections;
using System.Threading.Tasks;
using _Project.Scripts.Infrastructure.Services.AssetManagement;
using _Project.Scripts.Infrastructure.Services.Config;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Audio
{
    public class AudioService : IService, IInitializable
    {
        private readonly AssetProvider _assetProvider;
        private readonly ConfigService _configService;

        private AudioConfig _audioConfig;
        private AudioServiceView _audioServiceView;

        public AudioService(AssetProvider assetProvider, ConfigService configService)
        {
            _assetProvider = assetProvider;
            _configService = configService;
        }

        public async Task Initialize()
        {
            _audioConfig = _configService.Get<AudioConfig>();
            _audioServiceView = await _assetProvider.CreateAudioServiceView();
        }

        public void PlayClickSound() => _audioServiceView.PlaySound(_audioConfig.Click);

        public void PlayWinSound() => _audioServiceView.PlaySound(_audioConfig.Win);

        public void PlayLoseSound() => _audioServiceView.PlaySound(_audioConfig.Lose);

        public void PlayWindowShowSound() => _audioServiceView.PlaySound(_audioConfig.WindowShowSound);

        public void PlayMoneySound() => _audioServiceView.PlaySound(_audioConfig.Money);

        public void PlayLaunchSound() => _audioServiceView.PlaySound(_audioConfig.Launch);

        public void PlayHitSound() => _audioServiceView.PlaySound(_audioConfig.Hit);

        public void PlayDieSound() => _audioServiceView.PlaySound(_audioConfig.Die);

        public void PlayRingCollideSound() => _audioServiceView.PlaySound(_audioConfig.RingCollide);

        // public void PlayMusic() => _audioServiceView.PlaySound(_audioConfig.Music);

        public void PlayMusic(AudioClip music) => _audioServiceView.PlayMusic(music);

        public void MuteSound() => _audioServiceView.DisableSounds();

        public void UnmuteSound() => _audioServiceView.EnableSounds();

        public void MuteMusic() => _audioServiceView.DisableMusic();

        public void UnmuteMusic() => _audioServiceView.EnableMusic();
    }
}