using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;

namespace GameEventSystem.Editor
{
    public class GameEventView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<GameEventView, GraphView.UxmlTraits> { }

        public override bool supportsWindowedBlackboard => true;
        public MiniMap Minimap { get; private set; }
        public BlackboardView BlackboardView { get; private set; }
        public Blackboard Blackboard { get { return BlackboardView.blackboard; } private set { } }
        
        private GameEvent _gameEvent;
        private SerializedObject _serializedObject;
        private GameEventEditorWindow _window;

        private List<GameEventEditorNode> _graphNodes;
        private Dictionary<string, GameEventEditorNode> _nodeDictionary;
        private Dictionary<Edge, GameEventConnection> _connectionDictionary;

        public GameEventView()
        {
            StyleSheet style = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Assets/Scripts/GameEventSystem/Editor/Graph/USS/GameEventEditor.uss");
            styleSheets.Add(style);
            
            GridBackground background = new GridBackground();
            
            background.name = "Grid";
            Insert(0, background);
            
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());
            this.AddManipulator(new ContentZoomer());
        }

        public void InitGraph(SerializedObject serializedObject, GameEventEditorWindow window)
        {
            _gameEvent = (GameEvent)serializedObject.targetObject;
            _serializedObject = serializedObject;
            _window = window;

            _graphNodes = new List<GameEventEditorNode>();
            _nodeDictionary = new Dictionary<string, GameEventEditorNode>();
            _connectionDictionary = new Dictionary<Edge, GameEventConnection>();

            DrawBlackboard();
            DrawNodes();
            DrawConnections();

            graphViewChanged += OnGraphViewChanged;
            Undo.undoRedoPerformed += ReloadView;
        }
        
        public void Dispose()
        {
            graphViewChanged -= OnGraphViewChanged;
            Undo.undoRedoPerformed -= ReloadView;
        }

        private void ReloadView()
        {
            graphViewChanged -= OnGraphViewChanged;

            RemoveBlackboard();
            RemoveNodes();
            RemoveConnections();
            
            DrawBlackboard();
            DrawNodes();
            DrawConnections();
            
            graphViewChanged += OnGraphViewChanged;
        }

        // public void CreateMinimap(float windowWidth)
        // {
        //     Minimap = new MiniMap { anchored = true };
        //     Minimap.capabilities &= ~Capabilities.Movable;
        //     Minimap.SetPosition(new Rect(windowWidth - 210, 30, 200, 140));
        //     Add(Minimap);
        // }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> allPorts = new List<Port>();
            List<Port> ports = new List<Port>();

            foreach (var node in _graphNodes)
            {
                allPorts.AddRange(node.Ports);
            }

            foreach (var port in allPorts)
            {
                if (port == startPort) continue;

                if (port.node == startPort.node) continue;

                if (port.direction == startPort.direction) continue;

                if (port.portType != startPort.portType) continue;

                ports.Add(port);
            }

            return ports;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphviewchange)
        {
            if (graphviewchange.elementsToRemove != null)
            {
                Undo.RecordObject(_serializedObject.targetObject, "Removed elements");

                List<GameEventEditorNode> removedNodes = new List<GameEventEditorNode>();
                removedNodes.AddRange(graphviewchange.elementsToRemove.OfType<GameEventEditorNode>());
                
                if (removedNodes.Count > 0)
                {
                    for (int i = removedNodes.Count - 1; i >= 0; i--)
                    {
                        RemoveNode(removedNodes[i]);
                    }
                }

                List<Edge> removedEdges = new List<Edge>();
                removedEdges.AddRange(graphviewchange.elementsToRemove.OfType<Edge>());

                if (removedEdges.Count > 0)
                {
                    for (int i = removedEdges.Count - 1; i >= 0; i--)
                    {
                        RemoveConnection(removedEdges[i]);
                    }
                }
            }

            if (graphviewchange.movedElements != null)
            {
                Undo.RecordObject(_serializedObject.targetObject, "Moved elements");

                foreach (var editorNode in graphviewchange.movedElements.OfType<GameEventEditorNode>())
                {
                    editorNode.SavePosition();
                }
            }

            if (graphviewchange.edgesToCreate != null)
            {
                Undo.RecordObject(_serializedObject.targetObject, "Create connection");

                foreach (var edge in graphviewchange.edgesToCreate)
                {
                    CreateEdge(edge);
                }
            }
            
            EditorUtility.SetDirty(_gameEvent);

            return graphviewchange;
        }

        private void CreateEdge(Edge edge)
        {
            // edge.output and edge.input are reversed from what I would expect
            GameEventEditorNode inputNode = (GameEventEditorNode)edge.output.node;
            GameEventEditorNode outputNode = (GameEventEditorNode)edge.input.node;

            GameEventConnection connection =
                new GameEventConnection(inputNode.Id, outputNode.Id);

            _gameEvent.AllConnections.Add(connection);
            inputNode.AddConnection(connection);
            outputNode.AddConnection(connection);

            _connectionDictionary.Add(edge, connection);
        }

        private void DrawBlackboard()
        {
            BlackboardView = new BlackboardView();
            BlackboardView.gameEventView = this;
            BlackboardView.SetVisualGraph(_gameEvent);
            Blackboard.SetPosition(new Rect(10, 30, 250, 300));
            Add(BlackboardView.blackboard);
        }

        private void RemoveBlackboard()
        {
            Remove(BlackboardView.blackboard);
        }

        private void DrawNodes()
        {
            foreach (var node in _gameEvent.Nodes)
            {
                AddNodeToGraph(node);
            }
        }

        private void DrawConnections()
        {
            if (_gameEvent.AllConnections == null) return;
            foreach (var connection in _gameEvent.AllConnections)
            {
                AddConnection(connection);
            }
        }

        private void AddNode(GameEventNode node)
        {
            Undo.RecordObject(_serializedObject.targetObject, "Add Node");
            _gameEvent.Nodes.Add(node);
            _serializedObject.Update();

            AddNodeToGraph(node);
        }

        private void AddNodeToGraph(GameEventNode node)
        {
            node.TypeName = node.GetType().AssemblyQualifiedName;
            GameEventEditorNode editorNode = new GameEventEditorNode(node);
            editorNode.SetPosition(node.Position);
            editorNode.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Assets/Scripts/GameEventSystem/Editor/Graph/USS/Node.uss"));

            // IEnumerable<CustomNodeStyleAttribute> customStyleAttribs = node.GetType().GetCustomAttributes<CustomNodeStyleAttribute>();
            // if (customStyleAttribs != null)
            // {
            //     foreach (var customStyleAttrib in customStyleAttribs)
            //     {
            //         try
            //         {
            //             StyleSheet styleSheet =
            //                 AssetDatabase.LoadAssetAtPath<StyleSheet>(
            //                     $"Assets/Scripts/GameEventSystem/Editor/Graph/USS/{customStyleAttrib.style}.uss");
            //             if (styleSheet != null)
            //             {
            //                 editorNode.styleSheets.Add(styleSheet);
            //             }
            //             else throw new Exception();
            //         }
            //         catch (Exception ex)
            //         {
            //             Debug.LogWarning($"Style sheet does not exit: {customStyleAttrib.style}");
            //         }
            //     }
            // }

            _graphNodes.Add(editorNode);
            _nodeDictionary.Add(node.Id, editorNode);

            AddElement(editorNode);
        }

        private void RemoveNodes()
        {
            for (int i = _graphNodes.Count - 1; i >= 0; i--)
            {
                RemoveElement(_graphNodes[i]);
            }
            _graphNodes.Clear();
            _nodeDictionary.Clear();
        }
        
        private void RemoveConnections()
        {
            var allEdges = _connectionDictionary.Keys.ToList();
            for (int i = allEdges.Count - 1; i >= 0; i--)
            {
                RemoveElement(allEdges[i]);
            }
            _connectionDictionary.Clear();
        }

        private void RemoveNode(GameEventEditorNode editorNode)
        {
            _gameEvent.Nodes.Remove(editorNode.Node);
            _graphNodes.Remove(editorNode);
            _nodeDictionary.Remove(editorNode.Id);
            _serializedObject.Update();
        }

        private void AddConnection(GameEventConnection connection)
        {
            GameEventEditorNode inputNode = GetNode(connection.InputNodeId);
            GameEventEditorNode outputNode = GetNode(connection.OutputNodeId);

            if (inputNode == null || outputNode == null) return;

            Port inputPort = inputNode.OutputPort;
            Port outputPort = outputNode.InputPort;

            Edge edge = inputPort.ConnectTo(outputPort);

            AddElement(edge);

            _connectionDictionary.Add(edge, connection);
        }

        private void RemoveConnection(Edge edge)
        {
            if (!_connectionDictionary.ContainsKey(edge)) return;

            GameEventConnection connection = _connectionDictionary[edge];
            GetNode(connection.InputNodeId)?.RemoveConnection(connection);
            GetNode(connection.OutputNodeId)?.RemoveConnection(connection);
            _gameEvent.AllConnections.Remove(connection);
            _connectionDictionary.Remove(edge);
        }

        private GameEventEditorNode GetNode(string nodeId)
        {
            GameEventEditorNode node = null;
            _nodeDictionary.TryGetValue(nodeId, out node);

            return node;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            // base.BuildContextualMenu(evt);
            // return;

            #region Add Trigger Search Menu
            evt.menu.AppendAction("Add Trigger Node", eventAction =>
            {
                var provider = new TriggerNodeSearchProvider((type) =>
                {
                    var windowMousePosition =
                        this.ChangeCoordinatesTo(this, eventAction.eventInfo.mousePosition - _window.position.position);
                    var mousePosition = contentViewContainer.WorldToLocal(windowMousePosition);


                    var node = (GameEventNode)Activator.CreateInstance(type);
                    node.SetPosition(new Rect(mousePosition, new Vector2()));
                    AddNode(node);
                });
                SearchWindow.Open(new SearchWindowContext(eventAction.eventInfo.mousePosition),
                    provider);
            });
            #endregion

            #region Add Condition Search Menu
            evt.menu.AppendAction("Add Condition Node", eventAction =>
            {
                var provider = new ConditionNodeSearchProvider((type) =>
                {
                    var windowMousePosition =
                        this.ChangeCoordinatesTo(this, eventAction.eventInfo.mousePosition - _window.position.position);
                    var mousePosition = contentViewContainer.WorldToLocal(windowMousePosition);


                    var node = (GameEventNode)Activator.CreateInstance(type);
                    node.SetPosition(new Rect(mousePosition, new Vector2()));
                    AddNode(node);
                });
                SearchWindow.Open(new SearchWindowContext(eventAction.eventInfo.mousePosition),
                    provider);
            });
            #endregion
            
            #region Add Effect Search Menu
            evt.menu.AppendAction("Add Effect Node", eventAction =>
            {
                var provider = new EffectNodeSearchProvider((type) =>
                {
                    var windowMousePosition =
                        this.ChangeCoordinatesTo(this, eventAction.eventInfo.mousePosition - _window.position.position);
                    var mousePosition = contentViewContainer.WorldToLocal(windowMousePosition);


                    var node = (GameEventNode)Activator.CreateInstance(type);
                    node.SetPosition(new Rect(mousePosition, new Vector2()));
                    AddNode(node);
                });
                SearchWindow.Open(new SearchWindowContext(eventAction.eventInfo.mousePosition),
                    provider);
            });
            #endregion
        }
    }
}
