using System;

namespace _Project.Scripts.Data
{
    [Serializable]
    public class WorldData
    {
        public int CurrentLevel;

        public WorldData(int currentLevel) => CurrentLevel = currentLevel;
    }
}