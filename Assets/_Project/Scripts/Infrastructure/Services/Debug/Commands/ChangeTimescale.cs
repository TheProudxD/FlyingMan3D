using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Debug.Commands
{
    public class ChangeTimescale : DebugCommand<int>
    {
        public override string ID => "change_timescale";
        public override string Description => "Change Timescale";
        public override string Format => "change_timescale <int> [from 0 to 2]";
        
        public override void Invoke(int state) => Time.timeScale = state;
    }
}