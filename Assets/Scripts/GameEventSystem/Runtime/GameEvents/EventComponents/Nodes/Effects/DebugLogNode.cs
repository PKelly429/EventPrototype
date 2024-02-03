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
        [TextArea] [DisplayField] public string LogText;
        [DisplayField] public bool Test1;
        public bool Test2;

        protected override State OnUpdate()
        {
            Debug.Log(LogText);
            return State.Success;
        }
        
        
//         public override void DrawContent(VisualElement contentContainer)
//         {
// #if UNITY_EDITOR
//             PropertyField propertyField = new PropertyField();
//             propertyField.bindingPath = "LogText";
//             propertyField.BindProperty(new SerializedObject(this));
//             propertyField.label = string.Empty;
//             //propertyField.style.whiteSpace = WhiteSpace.Normal;
//             contentContainer.Add(propertyField);
// #endif
//         }
    }
}
