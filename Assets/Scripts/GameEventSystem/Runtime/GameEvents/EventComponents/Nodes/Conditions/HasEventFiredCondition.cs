using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameEventSystem
{
    [Serializable]
    [AddTypeMenu("Has Event Fired")]
    public class HasEventFiredCondition : AbstractCondition
    {
        public GameEvent otherEvent;
        public override bool CheckCondition(GameEvent parentEvent)
        {
            if (otherEvent == null) return false;
            return EventManager.HasEventTriggered(otherEvent);
        }
    }
}