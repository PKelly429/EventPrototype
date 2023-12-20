#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class EditorUtils
{
    public static int IndentSize = 5;
    
    public static void DrawUILine()
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(10 + 2));
        r.height = 2;
        r.y += 10 / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, Color.black);
    }
    
    public static GUIStyle UISubHeaderStyle
    {
        get
        {
            return new GUIStyle()
            {
                fontSize = 12,
                normal = new GUIStyleState()
                {
                    textColor = Color.white
                },
                alignment = TextAnchor.MiddleLeft
            };
        }
    }
}
#endif