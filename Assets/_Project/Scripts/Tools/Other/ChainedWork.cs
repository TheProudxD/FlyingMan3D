using System;
using System.Collections;
using System.Collections.Generic;

namespace _Project.Scripts.Tools.Other
{
    public class ChainedWork
    {
        private List<WorkRing> _chain = new();

        public IEnumerator DoWorkCo()
        {
            for (int i = 0; i < _chain.Count; i++)
            {
                yield return _chain[i].DoWorkRing();
            }

            yield return null;
        }

        public void AddWork(Action action, float delay = 0)
        {
            _chain.Add(new WorkRing(action, delay));
        }
    }
}