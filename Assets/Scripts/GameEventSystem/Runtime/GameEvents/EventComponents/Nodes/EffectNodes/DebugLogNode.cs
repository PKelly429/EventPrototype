using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace GameEventSystem
{
    [NodeInfo("Log", "z_DEBUG", 999)]
    [NodeDescription("Logs a message to the console")]
    public class DebugLogNode : EffectNode
    {
        public StringParameterBuilder text;

        protected override State OnUpdate()
        {
            Debug.Log(text.GetValue());
            
            return State.Success;
        }
    }
}
