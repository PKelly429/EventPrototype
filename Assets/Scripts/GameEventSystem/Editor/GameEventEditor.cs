using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace GameEventSystem.Editor
{
    [CustomEditor(typeof(GameEvent))]
    public class GameEventEditor : UnityEditor.Editor
    {
        private GUIStyle SubHeadingStyle = new GUIStyle();
        private List<VariableDefinition> removedVariables;
        private List<EventComponent> removedComponents;

        private Texture2D delTexture;
        private const float indentSize = 5f;
        private const float spaceSize = 10f;
        private bool showVariables = true;
        private bool showTriggers = true;
        private bool showConditions = true;
        private bool showEffects = true;
        

        private void OnEnable()
        {
            delTexture = EditorGUIUtility.FindTexture("Assets/Sprites/delete.png");
            removedVariables = new List<VariableDefinition>();
            removedComponents = new List<EventComponent>();
            SubHeadingStyle.fontSize = 16;
            SubHeadingStyle.fontStyle = FontStyle.Italic;
            SubHeadingStyle.normal.textColor = Color.white;
            SubHeadingStyle.alignment = TextAnchor.MiddleLeft;
            SubHeadingStyle.hover.textColor = Color.green;
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Editor"))
            {
                GameEventEditorWindow.Open((GameEvent)target);
            }

            DrawDefaultInspector();
            
            return;
            
        }

    }
}