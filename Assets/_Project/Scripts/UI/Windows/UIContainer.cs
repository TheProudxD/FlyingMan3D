using UnityEngine;

namespace _Project.Scripts.UI.Windows
{
    public abstract class UIContainer : MonoBehaviour
    {
        public abstract void Show();

        public abstract void Hide();
    }
}