using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace GameEventSystem
{
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

            for (int i = 0; i < parameters.Length; i++)
            {
                values[i] = parameters[i].GetValue();
            }

            return string.Format(text, values);
        }

        protected override void SetObjReferences()
        {
            foreach (var parameter in parameters)
            {
                parameter.ResolveRef(localBlackboard);
            }
        }

#if UNITY_EDITOR
        public override void DrawEditorWindowUI(IBlackboard localBlackboard)
        {
            text = EditorGUILayout.TextArea(text, new GUIStyle(EditorStyles.textArea) { wordWrap = true });
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
            if (parameters == null || parameters.Length != variables)
            {
                var old = parameters?.ToList();
                parameters = new StringParameter[variables];
                if (old != null)
                {
                    for (int i = 0; i < Mathf.Min(old.Count, parameters.Length); i++)
                    {
                        parameters[i] = old[i];
                    }
                }
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] == null)
                {
                    parameters[i] = new StringParameter();
                }

                EditorGUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"{i}:", GUILayout.Width(20));
                if (GUILayout.Button(parameters[i].GetName(), EditorStyles.linkLabel, GUILayout.ExpandWidth(false)))
                {
                    ShowEntitySearchWindow.Open(localBlackboard, parameters[i]);
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            try
            {
                // GUI.enabled = false;
                // EditorGUILayout.TextArea(ValidateValue(), new GUIStyle(EditorStyles.textArea){wordWrap =true});
                // GUI.enabled = true;
                string s = ValidateValue();
            }
            catch (Exception e)
            {
                EditorGUILayout.HelpBox("String cannot be formatted.", MessageType.Error);
            }

            EditorGUILayout.Space(5);
        }

        public string ValidateValue()
        {
            values = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                values[i] = parameters[i].GetName();
            }

            return string.Format(text, values);
        }
#endif
    }
}