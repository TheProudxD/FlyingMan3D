using System;
using UnityEngine.Serialization;

[Serializable]
public class UpperRing
{
    [FormerlySerializedAs("insideRings")] public RingData[] InsideRings = new RingData[2];
}