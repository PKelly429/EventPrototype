#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameEventSystem
{
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
        
        public static string GetDisplayString(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName)) return string.Empty;
			
            string str = fieldName.Replace("_", string.Empty);
            return $"{str[0].ToString().ToUpper()}{str.Substring(1)}";
        }
        
        /// <summary>
        /// Gets the object the property represents.
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static object GetTargetObjectOfProperty(SerializedProperty prop)
        {
            if (prop == null) return null;
            
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }
            return obj;
        }
        
        private static object GetValue_Imp(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();

            while (type != null)
            {
                var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                    return f.GetValue(source);

                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null)
                    return p.GetValue(source, null);

                type = type.BaseType;
            }
            return null;
        }
        
        private static object GetValue_Imp(object source, string name, int index)
        {
            var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
            if (enumerable == null) return null;
            var enm = enumerable.GetEnumerator();

            for (int i = 0; i <= index; i++)
            {
                if (!enm.MoveNext()) return null;
            }
            return enm.Current;
        }
    }
}
#endif
