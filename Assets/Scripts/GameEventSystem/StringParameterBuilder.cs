using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GameEventSystem.GameEvents.Editor;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class StringParameterBuilder : EventComponent
{
    public string text;
    public StringParameter[] parameters;
    private object[] values;
    
    public string GetValue()
    {
        if (values == null)
        {
            values = new object[parameters.Length];
        }

        for(int i=0;i<parameters.Length; i++)
        {
            values[i] = parameters[i].GetValue();
        }

        return string.Format(text, values);
    }

    public void ResolveRef()
    {
        foreach (var parameter in parameters)
        {
            parameter.ResolveRef(localBlackboard);
        }
    }

    #if UNITY_EDITOR
    public override void DrawEditorWindowUI()
    {
        text = EditorGUILayout.TextArea(text, new GUIStyle(EditorStyles.textArea){wordWrap =true});
        if (string.IsNullOrEmpty(text))
        {
            return;
        }
        Regex rx = new Regex("\\{.\\}");
        string[] words = rx.Split(text);
        if (words.Length == 0)
        {
            return;
        }
        int variables = words.Length - 1;
        if (parameters.Length != variables)
        {
            var old = parameters.ToList();
            parameters = new StringParameter[variables];
            for (int i = 0; i < Mathf.Min(old.Count, parameters.Length); i++)
            {
                parameters[i] = old[i];
            }
        }
        for(int i=0; i<parameters.Length; i++)
        {
            if (parameters[i] == null)
            {
                parameters[i] = new StringParameter();
            }
            if (GUILayout.Button(parameters[i].GetName(), EditorStyles.linkLabel, GUILayout.ExpandWidth(false)))
            {
                ShowEntitySearchWindow.Open(localBlackboard, new List<IBlackboard>(), parameters[i]);
            }
        }

        try
        {
            GUI.enabled = false;
            EditorGUILayout.TextArea(ValidateValue(), new GUIStyle(EditorStyles.textArea){wordWrap =true});
            GUI.enabled = true;
        }
        catch (Exception e)
        {
            EditorGUILayout.HelpBox("String cannot be formatted.", MessageType.Error);
        }
    }
    
    public string ValidateValue()
    {
        values = new object[parameters.Length];
        for(int i=0;i<parameters.Length; i++)
        {
            values[i] = parameters[i].GetName();
        }

        return string.Format(text, values);
    }
    #endif
}
