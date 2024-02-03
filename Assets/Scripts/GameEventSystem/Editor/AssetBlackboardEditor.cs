using System;
using System.Collections;
using System.Collections.Generic;
using GameEventSystem;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GameEventSystem.Editor
{
    [CustomEditor(typeof(AssetBlackboard))]
    public class AssetBlackboardEditor : UnityEditor.Editor
    {
        private GUIStyle SubHeadingStyle = new GUIStyle();
        private List<VariableDefinition> removedVariables;
        private Texture2D delTexture;
        private const float spaceSize = 10f;

        private void OnEnable()
        {
            delTexture = EditorGUIUtility.FindTexture("Assets/Sprites/delete.png");
            removedVariables = new List<VariableDefinition>();
            SubHeadingStyle.fontSize = 16;
            SubHeadingStyle.fontStyle = FontStyle.Italic;
            SubHeadingStyle.normal.textColor = Color.white;
            SubHeadingStyle.alignment = TextAnchor.MiddleLeft;
            SubHeadingStyle.hover.textColor = Color.green;
        }

        public override void OnInspectorGUI()
        {
            AssetBlackboard blackboard = (AssetBlackboard)target;
            serializedObject.Update();
            GUILayout.Label($"Variables", SubHeadingStyle);
            DrawBlackboardVariables(blackboard);
        }

        private void DrawBlackboardVariables(IBlackboard blackboard)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            foreach (var variableDefinition in blackboard.definedVariables)
            {
                EditorGUILayout.BeginHorizontal("box");
                variableDefinition.Name =
                    EditorGUILayout.TextField($"{variableDefinition.type}", variableDefinition.Name);
                if (GUILayout.Button(delTexture, EditorStyles.iconButton))
                {
                    removedVariables.Add(variableDefinition);
                }

                EditorGUILayout.EndHorizontal();
            }

            foreach (var removed in removedVariables)
            {
                blackboard.RemoveVariable(removed);
            }

            removedVariables.Clear();

            EditorGUILayout.Space(spaceSize);
            if (GUILayout.Button("Add Variable:", EditorStyles.popup))
            {
                var provider = new VariableDefinitionSearchProvider((type) =>
                {
                    blackboard.AddVariable(type);
                });
                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
                    provider);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
    }
}