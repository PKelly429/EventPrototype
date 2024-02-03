using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    [NodeInfo("Log")]
    public class DebugLogNode : EffectNode
    {
        public string LogText;

        protected override State OnUpdate()
        {
            Debug.Log(LogText);
            return State.Success;
        }
    }
}
