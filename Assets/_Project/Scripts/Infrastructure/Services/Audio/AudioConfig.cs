using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Audio
{
    [CreateAssetMenu(fileName = "Audios", menuName = "Configs/Audios", order = 0)]
    public class AudioConfig : Config.Config
    {
        [field: SerializeField] public AudioClip WindowShowSound { get; private set; }
        [field: SerializeField] public AudioClip Click { get; private set; }
        [field: SerializeField] public AudioClip Correct { get; private set; }
        [field: SerializeField] public AudioClip Fail { get; private set; }
        [field: SerializeField] public AudioClip Win { get; private set; }
        [field: SerializeField] public AudioClip Lose { get; private set; }
        [field: SerializeField] public AudioClip HitStar { get; private set; }

        public IEnumerable<AudioClip> GetAll() =>
            typeof(AudioConfig)
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(field => field.FieldType == typeof(AudioClip))
                .Select(field => (AudioClip)field.GetValue(this));
    }
}