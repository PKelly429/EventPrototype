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
    [NodeInfo("Log", "")]
    [NodeDescription("Logs a message to the console")]
    public class DebugLogNode : EffectNode
    {
        [TextArea] public string logText;
        public BoolParameter _testBool;
        public StringParameter text;

        protected override State OnUpdate()
        {
            Debug.Log(logText);
            
            return State.Success;
        }
    }
}
