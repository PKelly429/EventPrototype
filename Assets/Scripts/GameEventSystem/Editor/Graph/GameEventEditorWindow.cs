using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace GameEventSystem.Editor
{
    public class GameEventEditorWindow : EditorWindow
    {
        private static GameEventEditorWindow _instance;
        
        [SerializeField] private GameEvent _currentEvent;
        [SerializeField] private SerializedObject _serializedObject;
        [SerializeField] private GameEventView _currentView;
        [SerializeField] private BlackboardView _blackboardView;
        [SerializeField] private InspectorView _inspectorView;
        [SerializeField] private ToolbarMenu _toolbarMenu;
        
        [SerializeField] private TextField _newEventNameField;
        [SerializeField] private TextField _locationPathField;
        [SerializeField] private Button _createNewButton;
        [SerializeField] private VisualElement _overlay;

        public GameEvent CurrentEvent => _currentEvent;

        public static void Open(GameEvent target)
        {
            if (_instance != null)
            {
                _instance.Focus();
                _instance.Load(target);
            }
            GameEventEditorWindow[] windows = Resources.FindObjectsOfTypeAll<GameEventEditorWindow>();
            foreach (var window in windows)
            {
                if(window == null) continue;
                _instance = window;
                _instance.Focus();
                _instance.Load(target);
                return;
            }

            _instance = CreateWindow<GameEventEditorWindow>(typeof(GameEventEditorWindow), typeof(SceneView));
            _instance.titleContent = new GUIContent($"{target.name}", EditorGUIUtility.ObjectContent(null, typeof(GameEvent)).image);
            _instance._currentEvent = target;
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
            _blackboardView = rootVisualElement.Q<BlackboardView>();
            _inspectorView = rootVisualElement.Q<InspectorView>();
            
            //Blackboard
            _blackboardView.SetupBlackboard();
            
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
            _toolbarMenu.menu.AppendAction("New GameEvent...", (a) => CreateNewGameEvent("NewEvent"));
            
            // New Tree Dialog
            _newEventNameField = rootVisualElement.Q<TextField>("EventName");
            _locationPathField = rootVisualElement.Q<TextField>("LocationPath");
            _overlay = rootVisualElement.Q<VisualElement>("Overlay");
            _createNewButton = rootVisualElement.Q<Button>("CreateButton");
            _createNewButton.clicked += () => CreateNewGameEvent(_newEventNameField.value);
            
            _currentView.OnNodeSelected += OnNodeSelectionChanged;
            _currentView.OnNodeUnselected += OnNodeUnselected;
            
            if(_currentEvent != null) Load(_currentEvent);
        }
        
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int index)
        {
            Object asset = EditorUtility.InstanceIDToObject(instanceId);
            if (asset.GetType() == typeof(GameEvent))
            {
                GameEventEditorWindow.Open((GameEvent)asset);
                return true;
            }

            return false;
        }

        List<T> LoadAssets<T>() where T : UnityEngine.Object 
        {
            string[] assetIds = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            List<T> assets = new List<T>();
            foreach (var assetId in assetIds) 
            {
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
                    OnSelectionChange();
                    break;
            }
        }

        private void OnSelectionChange()
        {
            EditorApplication.delayCall += () =>
            {
                if (_currentView == null) return;
                
                if (_currentEvent == null)
                {
                    if(_overlay !=null) _overlay.style.visibility = Visibility.Visible;
                    _currentView.ClearView();
                }

                if (_inspectorView == null) return;

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
                    else
                    {
                        AssetBlackboard blackboard = Selection.activeObject as AssetBlackboard;
                        if (blackboard)
                        {
                            Load(null);
                            _blackboardView.SetBlackboard(blackboard);
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
            _blackboardView.SetGameEvent(target);
            DrawGraph();
            if(_currentEvent != null) _overlay.style.visibility = Visibility.Hidden;
        }

        private void DrawGraph()
        {
            if (_currentView == null) return;
            if (_currentEvent == null)
            {
                _currentView.ClearView();
                titleContent = new GUIContent($"(none)", EditorGUIUtility.ObjectContent(null, typeof(GameEvent)).image);
                return;
            }

            titleContent = new GUIContent($"{_currentEvent.name}", EditorGUIUtility.ObjectContent(null, typeof(GameEvent)).image);
            _serializedObject = new SerializedObject(_currentEvent);
            _currentView.PopulateView(_serializedObject, this);
        }
        
        void CreateNewGameEvent(string assetName) 
        {
            string path = System.IO.Path.Combine(_locationPathField.value, $"{assetName}.asset");
            path = AssetDatabase.GenerateUniqueAssetPath(path);
            GameEvent newEvent = ScriptableObject.CreateInstance<GameEvent>();
            newEvent.name = _newEventNameField.ToString();
            AssetDatabase.CreateAsset(newEvent, path);
            AssetDatabase.SaveAssets();
            Selection.activeObject = newEvent;
            EditorGUIUtility.PingObject(newEvent);
        }
    }
}
