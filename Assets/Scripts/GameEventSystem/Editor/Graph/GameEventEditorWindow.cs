using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameEventSystem.Editor
{
    public class GameEventEditorWindow : EditorWindow
    {
        [SerializeField] private GameEvent _currentEvent;
        [SerializeField] private SerializedObject _serializedObject;
        [SerializeField] private GameEventView _currentView;
        [SerializeField] private InspectorView _inspectorView;
        [SerializeField] private ToolbarMenu _toolbarMenu;
        
        [SerializeField] private TextField _newEventNameField;
        [SerializeField] private TextField _locationPathField;
        [SerializeField] private Button _createNewButton;
        [SerializeField] private VisualElement _overlay;

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

        public void CreateGUI()
        {
            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/GameEventSystem/Editor/Graph/USS/GameEventEditor.uxml");
            visualTree.CloneTree(rootVisualElement);
        
            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/GameEventSystem/Editor/Graph/USS/GameEventEditor.uss");
            rootVisualElement.styleSheets.Add(styleSheet);

            _currentView = rootVisualElement.Q<GameEventView>();
            _inspectorView = rootVisualElement.Q<InspectorView>();
            
            // Toolbar assets menu
            _toolbarMenu = rootVisualElement.Q<ToolbarMenu>();
            var behaviourTrees = LoadAssets<GameEvent>();
            behaviourTrees.ForEach(tree => 
            {
                _toolbarMenu.menu.AppendAction($"{tree.name}", (a) => 
                {
                    Selection.activeObject = tree;
                });
            });
            _toolbarMenu.menu.AppendSeparator();
            _toolbarMenu.menu.AppendAction("New Tree...", (a) => CreateNewTree("NewEvent"));
            
            // New Tree Dialog
            _newEventNameField = rootVisualElement.Q<TextField>("EventName");
            _locationPathField = rootVisualElement.Q<TextField>("LocationPath");
            _overlay = rootVisualElement.Q<VisualElement>("Overlay");
            _createNewButton = rootVisualElement.Q<Button>("CreateButton");
            _createNewButton.clicked += () => CreateNewTree(_newEventNameField.value);
            
            _currentView.OnNodeSelected += OnNodeSelectionChanged;
            _currentView.OnNodeUnselected += OnNodeUnselected;
        }
        
        List<T> LoadAssets<T>() where T : UnityEngine.Object {
            string[] assetIds = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            List<T> assets = new List<T>();
            foreach (var assetId in assetIds) {
                string path = AssetDatabase.GUIDToAssetPath(assetId);
                T asset = AssetDatabase.LoadAssetAtPath<T>(path);
                assets.Add(asset);
            }
            return assets;
        }

        private void OnNodeUnselected(GameEventNodeView nodeView)
        {
            _inspectorView.HandleNodeUnselected(nodeView);
        }
        
        private void OnNodeSelectionChanged(GameEventNodeView nodeView)
        {
            _inspectorView.UpdateSelection(nodeView);
        }

        private void OnEnable()
        {
            OnSelectionChange();
            
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable() 
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }
        
        private void OnInspectorUpdate() 
        {
            _currentView?.UpdateNodeStates();
        }
        
        private void OnPlayModeStateChanged(PlayModeStateChange obj) 
        {
            switch (obj) {
                case PlayModeStateChange.EnteredEditMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
            }
        }

        private void OnSelectionChange()
        {
            EditorApplication.delayCall += () =>
            {
                if (_inspectorView == null) return;
                
                // _overlay.style.visibility = Visibility.Visible;
                // _currentView.ClearView();
                
                GameEvent gameEvent = Selection.activeObject as GameEvent;
                if (!gameEvent)
                {
                    if (Selection.activeGameObject)
                    {
                        GameEventRunner runner = Selection.activeGameObject.GetComponent<GameEventRunner>();
                        if (runner)
                        {
                            gameEvent = runner.GameEvent;
                        }
                    }
                }

                if (!gameEvent) return;

                if (Application.isPlaying || AssetDatabase.CanOpenAssetInEditor(gameEvent.GetInstanceID()))
                {
                    Load(gameEvent);
                }
            };
        }

        public void Load(GameEvent target)
        {
            _currentEvent = target;
            _inspectorView.UpdateSelection(null);
            DrawGraph();
            _overlay.style.visibility = Visibility.Hidden;
        }

        private void DrawGraph()
        {
            if (_currentEvent == null) return;
            if (_currentView == null) return;
            
             _serializedObject = new SerializedObject(_currentEvent);
             _currentView.PopulateView(_serializedObject, this);
        }
        
        void CreateNewTree(string assetName) 
        {
            string path = System.IO.Path.Combine(_locationPathField.value, $"{assetName}.asset");
            GameEvent newEvent = ScriptableObject.CreateInstance<GameEvent>();
            newEvent.name = _newEventNameField.ToString();
            AssetDatabase.CreateAsset(newEvent, path);
            AssetDatabase.SaveAssets();
            Selection.activeObject = newEvent;
            EditorGUIUtility.PingObject(newEvent);
        }
    }
}
