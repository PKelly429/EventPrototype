using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameEventSystem.Editor
{
    public class GameEventEditorWindow : EditorWindow
    {
        [SerializeField] private GameEvent _currentEvent;
        [SerializeField] private SerializedObject _serializedObject;
        [SerializeField] private GameEventView _currentView;

        public GameEvent CurrentEvent => _currentEvent;

        public static void Open(GameEvent target)
        {
            GameEventEditorWindow[] windows = Resources.FindObjectsOfTypeAll<GameEventEditorWindow>();
            foreach (var window in windows)
            {
                if (window.CurrentEvent == target)
                {
                    window.Focus();
                    return;
                }
            }

            GameEventEditorWindow newWindow =
                CreateWindow<GameEventEditorWindow>(typeof(GameEventEditorWindow), typeof(SceneView));
            newWindow.titleContent = new GUIContent($"{target.name}", EditorGUIUtility.ObjectContent(null, typeof(GameEvent)).image);
            newWindow.Load(target);
        }

        public void Load(GameEvent target)
        {
            _currentEvent = target;
            DrawGraph();
        }

        private void DrawGraph()
        {
            _serializedObject = new SerializedObject(_currentEvent);
            _currentView = new GameEventView(_serializedObject, this);
            rootVisualElement.Add(_currentView);
        }
    }
}
