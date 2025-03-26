using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Audio
{
    public class AudioServiceView : MonoBehaviour
    {
        [SerializeField] private AudioSource _soundAudioSource;
        [SerializeField] private AudioSource _musicAudioSource;
    
        private void Awake() => DontDestroyOnLoad(gameObject);

        public void PlaySound(AudioClip clip) => _soundAudioSource.PlayOneShot(clip);
    
        public void PlayMusic(AudioClip clip) => _musicAudioSource.PlayOneShot(clip);
        
        public void DisableSounds() => _soundAudioSource.mute = true;

        public void EnableSounds() => _soundAudioSource.mute = false;
    
        public void DisableMusic() => _musicAudioSource.mute = true;

        public void EnableMusic() => _musicAudioSource.mute = false;
    }
}