using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(GameEvent))]
public class GameEventEditor : Editor
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
        GameEvent gameEvent = (GameEvent)target;
        serializedObject.Update();
        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
        //EditorGUILayout.LabelField(gameEvent.uniqueID.ToString());
        
        //DrawPropertiesExcluding(serializedObject, "triggers");

        DrawVariables(gameEvent);
        

        EditorGUILayout.Space(spaceSize);
        DrawTriggers(gameEvent);

        EditorGUILayout.Space(spaceSize);
        DrawEffects(gameEvent);
        
        EditorGUILayout.Space(spaceSize);
        if (GUILayout.Button("Fire Event"))
        {
            gameEvent.FireEvent();
        }
        
        EditorGUILayout.EndVertical();
        UnityEditor.EditorUtility.SetDirty(gameEvent);
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawVariables(GameEvent gameEvent)
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("blackboards"));
        string prefix = showVariables ? "[-]" : "[+]";
        if (GUILayout.Button($"{prefix} Variables", SubHeadingStyle))
        {
            showVariables = !showVariables;
        }
        if (showVariables)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(indentSize);
            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            foreach (var variableDefinition in gameEvent.definedVariables)
            {
                EditorGUILayout.BeginHorizontal("box");
                variableDefinition.name = EditorGUILayout.TextField($"{variableDefinition.type}", variableDefinition.name);
                if (GUILayout.Button(delTexture, EditorStyles.iconButton))
                {
                    removedVariables.Add(variableDefinition);
                }

                EditorGUILayout.EndHorizontal();
            }

            foreach (var removed in removedVariables)
            {
                gameEvent.definedVariables.Remove(removed);
            }

            removedVariables.Clear();

            EditorGUILayout.Space(spaceSize);
            if (GUILayout.Button("Add Variable:", EditorStyles.popup))
            {
                var provider = new VariableDefinitionSearchProvider((type) =>
                {
                    gameEvent.AddVariable((VariableDefinition)Activator.CreateInstance(type));
                });
                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
                    provider);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawTriggers(GameEvent gameEvent)
    {
        var prefix = showTriggers ? "[-]" : "[+]";
        if (GUILayout.Button($"{prefix} Triggers ({gameEvent.triggers.Count})", SubHeadingStyle))
        {
            showTriggers = !showTriggers;
        }
        
        if (showTriggers)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(indentSize);
            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            foreach (var trigger in gameEvent.triggers)
            {
                EditorGUILayout.BeginHorizontal("box");
                EditorGUILayout.BeginVertical();
                trigger.DrawEditorWindowUI(gameEvent);
                EditorGUILayout.EndVertical();
                if (GUILayout.Button(delTexture, EditorStyles.iconButton))
                {
                    removedComponents.Add(trigger);
                }

                EditorGUILayout.EndHorizontal();
            }

            foreach (var removed in removedComponents)
            {
                gameEvent.triggers.Remove((Trigger)removed);
            }

            removedComponents.Clear();

            EditorGUILayout.Space(spaceSize);
            if (GUILayout.Button("Add Trigger:", EditorStyles.popup))
            {
                var provider = new TriggerSearchProvider((type) =>
                {
                    gameEvent.AddTrigger((Trigger)Activator.CreateInstance(type));
                });
                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
                    provider);
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }
    }
    
    private void DrawEffects(GameEvent gameEvent)
    {
        var prefix = showEffects ? "[-]" : "[+]";
        if (GUILayout.Button($"{prefix} Effects ({gameEvent.triggers.Count})", SubHeadingStyle))
        {
            showEffects = !showEffects;
        }
        
        if (showEffects)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(indentSize);
            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            DrawComponentList(gameEvent, gameEvent.effects);

            EditorGUILayout.Space(spaceSize);
            if (GUILayout.Button("Add Effect:", EditorStyles.popup))
            {
                var provider = new EffectSearchProvider((type) =>
                {
                    gameEvent.AddEffect((Effect)Activator.CreateInstance(type));
                });
                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
                    provider);
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawComponentList<T>(GameEvent gameEvent, List<T> components) where T : EventComponent
    {
        foreach (var component in components)
        {
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.BeginVertical();
            component.DrawEditorWindowUI(gameEvent);
            EditorGUILayout.EndVertical();
            if (GUILayout.Button(delTexture, EditorStyles.iconButton))
            {
                removedComponents.Add(component);
            }

            EditorGUILayout.EndHorizontal();
        }

        foreach (var removed in removedComponents)
        {
            components.Remove((T)removed);
        }

        removedComponents.Clear();
    }

    private static void DrawUILine()
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(10 + 2));
        r.height = 2;
        r.y += 10 / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, Color.black);
    }
    
}
