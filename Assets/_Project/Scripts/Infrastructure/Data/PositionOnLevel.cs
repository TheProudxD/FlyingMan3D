using System;

namespace _Project.Scripts.Data
{
    [Serializable]
    public class PositionOnLevel
    {
        public string Level;
        public Vector3Data Position;

        public PositionOnLevel(string level, Vector3Data position = default)
        {
            Level = level;
            Position = position;
        }
    }
}