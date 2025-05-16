using System;
using System.Linq;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.LevelSystem
{
    [CreateAssetMenu(fileName = "LevelContainer", menuName = "Configs/LevelContainer", order = 0)]
    public class LevelContainer : Config.Config
    {
        [SerializeField] private Level[] _levels;

        private void OnEnable()
        {
            if (_levels.Any(l => l == null))
                UnityEngine.Debug.LogError("Level is null");
        }

        public int LevelsCount => _levels.Length;

        public Level this[int currentLevel] => _levels[(currentLevel - 1) % LevelsCount];
    }
}