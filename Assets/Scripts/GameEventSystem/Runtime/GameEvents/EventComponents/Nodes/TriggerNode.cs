using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    public abstract class TriggerNode : GameEventNode
    {
        [SerializeField] private bool _repeat;
        public override void Setup()
        {
            AddListener();
            state = State.Running;
        }

        protected override State OnUpdate()
        {
            return state;
        }

        protected abstract void AddListener();
        protected abstract void RemoveListener();

        protected void Trigger()
        {
            if (!_repeat)
            {
                state = State.Success;
                RemoveListener();
            }
            foreach (var node in Outputs)
            {
                node.Execute();
            }
        }
    }
}
