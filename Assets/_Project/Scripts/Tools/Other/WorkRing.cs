using System;
using System.Collections;
using UnityEngine;

namespace _Project.Scripts.Tools.Other
{
    public class WorkRing
    {
        private readonly Action _action;
        private readonly float _delay;

        public WorkRing(Action action, float delay)
        {
            _action = action;
            _delay = delay;
        }

        public IEnumerator DoWorkRing()
        {
            yield return new WaitForSeconds(_delay);
            PopulateAction(_action);
        }

        public void PopulateAction(Action action)
        {
            action ??= () => Debug.Log("Null Callback");
            action();
        }
    }
}