using _Project.Scripts.Infrastructure.Services;
using Reflex.Attributes;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.UI.Views
{
    public abstract class BaseView : MonoBehaviour
    {
        [Inject] protected AnimationService AnimationService;

        [SerializeField] protected TextMeshProUGUI Text;
        [SerializeField] protected float IncrementDuration = 0.5f;
    }
}