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
        public override bool ShouldUpdateWhenRunning => false;

        public void AddTriggerListener()
        {
            SetState(State.Running);
            AddListener();
        }
        public void RemoveTriggerListener()
        {
            RemoveListener();
            
            if(state != State.Success) SetState(State.Failure);
        }

        protected override State OnUpdate()
        {
            return state;
        }

        protected abstract void AddListener();
        protected abstract void RemoveListener();

        protected void Trigger()
        {
            if (runtimeGameEvent.CheckConditions())
            {
                SetState(State.Success);
                runtimeGameEvent.FireEvent();
            }
        }
    }
}
