using UnityEngine;

namespace _Project.Scripts.Gameplay
{
    public class HintStar : MonoBehaviour
    {
        public void Enable()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }

        public void Disable()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}