using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts.Gameplay
{
    [Serializable]
    public class RingData
    {
        [FormerlySerializedAs("ringType")] public RingType RingType;
        [FormerlySerializedAs("effect")] public int Effect;
        public Vector3 MovementAxis;
        public float Speed;
    }
}
