using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    [NodeInfo("Emergent Event", "")]
    [NodeDescription("Triggered by event scheduler if conditions met")]
    public class EmergentEventTrigger : TriggerNode
    {
        protected override void AddListener()
        {
            EventManager.RegisterEmergentEvent(_runtimeGameEvent);
        }

        protected override void RemoveListener()
        {
            state = State.Success;
            EventManager.RemoveEmergentEvent(_runtimeGameEvent);
        }
    }
}