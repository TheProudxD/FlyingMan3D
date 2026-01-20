using _Project.Scripts.Gameplay;
using BhorGames.Mechanics;

namespace _Project.Scripts.Infrastructure.Services.Scene
{
    public sealed class LevelSceneReferences
    {
        public LevelSceneReferences(Indicator indicator, Spawner spawner, Platform platform)
        {
            Indicator = indicator;
            Spawner = spawner;
            Platform = platform;
        }

        public Indicator Indicator { get; }
        public Spawner Spawner { get; }
        public Platform Platform { get; }
        public Slingshot Slingshot { get; set; } // Set dynamically after creation
    }
}
