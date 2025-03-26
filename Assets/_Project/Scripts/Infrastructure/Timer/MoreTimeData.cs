using UnityEngine;

namespace _Project.Scripts.SO
{
    [CreateAssetMenu(menuName = "Configs/GetMoreTimeData", fileName = "GetMoreTimeData Config", order = 0)]
    public class MoreTimeData : ScriptableObject
    {
        [field: SerializeField] public int AdditionalTimePrice { get; private set; }
        [field: SerializeField] public int AdditionalTime { get; private set; }
    }
}