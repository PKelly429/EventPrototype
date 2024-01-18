using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameEventSystem.Editor
{
    public class GameEventView : GraphView
    {
        private GameEvent _gameEvent;
        private SerializedObject _serializedObject;
        private GameEventEditorWindow _window;

        private List<GameEventEditorNode> _graphNodes;
        private Dictionary<string, GameEventEditorNode> _nodeDictionary;

        private NodeSearchProvider _nodeSearchProvider;

        public GameEventView(SerializedObject serializedObject, GameEventEditorWindow window)
        {
            _gameEvent = (GameEvent)serializedObject.targetObject;
            _serializedObject = serializedObject;
            _window = window;

            _graphNodes = new List<GameEventEditorNode>();
            _nodeDictionary = new Dictionary<string, GameEventEditorNode>();

            GridBackground background = new GridBackground();

            StyleSheet style = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    "Assets/Scripts/GameEventSystem/Editor/Graph/USS/GameEventEditor.uss");
            styleSheets.Add(style);
            background.name = "Grid";
            Add(background);
            background.SendToBack();
            
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());
            
            _nodeSearchProvider = new NodeSearchProvider((type) =>
            {
                //gameEvent.AddTrigger((GameEventNode)Activator.CreateInstance(type));
                var windowMousePosition =
                    this.ChangeCoordinatesTo(this, _nodeSearchProvider.MousePosition - window.position.position);
                var mousePosition = contentViewContainer.WorldToLocal(windowMousePosition);

                var node = (GameEventNode)Activator.CreateInstance(type);
                node.SetPosition(new Rect(mousePosition, new Vector2()));
                AddNode(node);
            });
            
            this.nodeCreationRequest = NodeCreationRequest;
        }

        private void NodeCreationRequest(NodeCreationContext obj)
        {
            _nodeSearchProvider.MousePosition = obj.screenMousePosition;
            SearchWindow.Open(new SearchWindowContext(obj.screenMousePosition), _nodeSearchProvider);
        }

        private void AddNode(GameEventNode node)
        {
            Undo.RecordObject(_serializedObject.targetObject, "Add Node");
            _gameEvent._nodes.Add(node);
            _serializedObject.Update();

            AddNodeToGraph(node);
        }

        private void AddNodeToGraph(GameEventNode node)
        {
            node.TypeName = node.GetType().AssemblyQualifiedName;
            GameEventEditorNode editorNode = new GameEventEditorNode();
            editorNode.SetPosition(node.Position);
            
            _graphNodes.Add(editorNode);
            _nodeDictionary.Add(node.Id, editorNode);
            
            AddElement(editorNode);
        }
    }
}
