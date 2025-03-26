using System.Collections.Generic;
using LitMotion;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.UI.Views
{
    public class LivesTrackerView : BaseView
    {
        [SerializeField] private GameObject _heartPrefab;

        private readonly Stack<GameObject> _hearts = new();

        public void DisplayHearts(int livesNumber)
        {
            if (_hearts.Count < livesNumber)
                CreateHearts(livesNumber);
            else
                DestroyHearts(livesNumber);
        }

        private void CreateHearts(int livesNumber)
        {
            for (int i = _hearts.Count; i < livesNumber; i++)
            {
                GameObject heart = Instantiate(_heartPrefab, transform);
                _hearts.Push(heart);
                heart.name = "Heart " + i;
            }
        }

        private void DestroyHearts(int livesNumber)
        {
            for (int i = _hearts.Count; i > livesNumber; i--)
            {
                if (CheckIfAreOutOfHearts())
                    return;

                GameObject last = _hearts.Pop();

                AnimationService.Scale(last.transform, last.transform.localScale, Vector3.zero, 0.25f,
                    callback: () => Destroy(last));
            }

            CheckIfAreOutOfHearts();
        }

        private bool CheckIfAreOutOfHearts() => _hearts.Count == 0;
    }
}