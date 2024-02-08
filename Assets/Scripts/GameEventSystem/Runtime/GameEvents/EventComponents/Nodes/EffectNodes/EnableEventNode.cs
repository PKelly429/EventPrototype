using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    [NodeInfo("Enable Event", "EVENT")]
    public class EnableEventNode : EffectNode
    {
        [DisplayField] public GameEvent gameEvent;

        protected override State OnUpdate()
        {
            EventManager.EnableGameEvent(gameEvent);
            
            return State.Success;
        }
    }
}
