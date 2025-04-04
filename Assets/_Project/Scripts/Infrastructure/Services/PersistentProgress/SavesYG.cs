using System;

namespace YG
{
    public partial class SavesYG
    {
        public int level = 1;
        public int richestLevel = -1;
        public int money = 250;

        public int health = 1;
        public int healthProgress = 1;
        [NonSerialized] public int healthDelta = 1;

        public float movingSpeed = 0.25f;
        public int movingSpeedProgress = 1;
        [NonSerialized] public float movingSpeedDelta = 0.05f;

        public float flyingControl = 15;
        public int flyingControlProgress = 1;
        [NonSerialized] public float flyingControlDelta = 5f;
    }
}