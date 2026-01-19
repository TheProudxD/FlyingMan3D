using _Project.Scripts.Infrastructure.FSM;
using Reflex.Attributes;
using Reflex.Core;
using UnityEngine;

namespace _Project.Scripts.Infrastructure
{
    public class GameBootstraper : MonoBehaviour
    {
        [Inject] private Container _container;

        private void Start()
        {
            StartGame();
        }

        private void StartGame() => _container.Resolve<StateMachine>().Initialize();
    }
}