#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace GameEventSystem
{
    namespace GameEventSystem.GameEvents.Editor
    {
        public static class EventEditorUtils
        {
            public static void DrawLinkText(string text, IBlackboard blackboard, params VariableReference[] variables)
            {
                if (string.IsNullOrEmpty(text))
                {
                    return;
                }

                Regex rx = new Regex("\\{.\\}");
                string[] words = rx.Split(text);
                if (words.Length != variables.Length + 1)
                {
                    Debug.LogError("Text does not contain correct number of params");
                    return;
                }

                using (var h = new GUILayout.HorizontalScope())
                {
                    int i = 0;
                    while (i < variables.Length)
                    {
                        GUILayout.Label(words[i], GUILayout.ExpandWidth(false));
                        if (GUILayout.Button(variables[i].GetName(), EditorStyles.linkLabel,
                            GUILayout.ExpandWidth(false)))
                        {
                            ShowEntitySearchWindow.Open(blackboard, variables[i]);
                        }

                        i++;
                    }

                    if (i < words.Length) GUILayout.Label(words[i], GUILayout.ExpandWidth(false));
                }

            }
        }
    }
}
#endif