using _Project.Scripts.Infrastructure.FSM;
using Reflex.Attributes;
using Reflex.Core;
using UnityEngine;
using YG;

namespace _Project.Scripts.Infrastructure
{
    public class GameBootstraper : MonoBehaviour
    {
        [Inject] private Container _container;

        private void OnEnable() => YG2.onGetSDKData += StartGame;

        private void OnDisable() => YG2.onGetSDKData -= StartGame;

        private void Awake()
        {
            if (YG2.isSDKEnabled)
                StartGame();
        }

        private void StartGame() => _container.Resolve<StateMachine>().Initialize();
    }
}