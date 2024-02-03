using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    [NodeInfo("On Start", "", 0, false, true)]
    public class OnStartTrigger : TriggerNode
    {
        protected override void AddListener()
        {
            Trigger();
        }

        protected override void RemoveListener()
        {
            
        }
    }
}
