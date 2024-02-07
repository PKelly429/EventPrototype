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
    [NodeInfo("Log", "DEBUG", 999)]
    [NodeDescription("Logs a message to the console")]
    public class DebugLogNode : EffectNode
    {
        //[DisplayField] public StringParameter text;
        public StringParameterBuilder text;

        protected override State OnUpdate()
        {
            Debug.Log(text.GetValue());
            
            return State.Success;
        }
    }
}
