using System;
using UnityEngine.Serialization;

namespace _Project.Scripts.Gameplay
{
    [Serializable]
    public class UpperRing
    {
        [FormerlySerializedAs("insideRings")] public RingData[] InsideRings = new RingData[2];
    }
}
