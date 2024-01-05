using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameEventSystem
{
    [Serializable]
    public class CheckVariable : Condition
    {
        public BoolParameter reference;
        public bool value;

        public override bool Evaluate()
        {
            return reference.GetValue() == value;
        }

#if UNITY_EDITOR
        public override void DrawEditorWindowUI(IBlackboard localBlackboard)
        {
            value = EditorGUILayout.Toggle("Value", value);
        }
#endif
    }
}