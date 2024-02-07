using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    [NodeConnectionInput(PortTypeDefinitions.PortTypes.Flow, 0)]
    [NodeConnectionOutput(PortTypeDefinitions.PortTypes.Trigger)]
    public abstract class TriggerNode : GameEventNode
    {
        public override bool IsTriggerNode => true;

        public void AddTriggerListener()
        {
            state = State.Running;
            AddListener();
        }
        public void RemoveTriggerListener()
        {
            RemoveListener();
            
            if(state != State.Success) state = State.Failure;
        }

        protected override State OnUpdate()
        {
            return state;
        }

        protected abstract void AddListener();
        protected abstract void RemoveListener();

        protected void Trigger()
        {
            if (_runtimeGameEvent.CheckConditions())
            {
                state = State.Success;
                _runtimeGameEvent.FireEvent();
            }
        }
    }
}
