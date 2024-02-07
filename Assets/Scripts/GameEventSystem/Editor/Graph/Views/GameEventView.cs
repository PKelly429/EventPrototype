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
    public class GameEventView : GraphView, IDisposable
    {
        public new class UxmlFactory : UxmlFactory<GameEventView, GraphView.UxmlTraits> { }
        
        public Action<GameEventNodeView> OnNodeSelected;
        public Action<GameEventNodeView> OnNodeUnselected;

        public override bool supportsWindowedBlackboard => true;

        private GameEvent _gameEvent;
        private SerializedObject _serializedObject;
        private GameEventEditorWindow _window;

        private List<GameEventNodeView> _graphNodes;
        private Dictionary<string, GameEventNodeView> _nodeDictionary;
        private Dictionary<Edge, GameEventConnection> _connectionDictionary;
        private Dictionary<GameEventConnection, Edge> _edgeDictionary;
        private Dictionary<GameEventNodeView, GameEventConnection> _nodeInputConnections; // we keep track of these to remove custom ports

        private bool _reloadView;

        # region Static Helper Methods
        private static GameEventView _currentGameEventView;

        public static void AddChildButton(Type type, Vector2 mousePos)
        {
            if (_currentGameEventView == null) return;
            _currentGameEventView.AddNode(type, mousePos);
        }
        #endregion
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
            
            _graphNodes = new List<GameEventNodeView>();
            _nodeDictionary = new Dictionary<string, GameEventNodeView>();
            _connectionDictionary = new Dictionary<Edge, GameEventConnection>();
            _edgeDictionary = new Dictionary<GameEventConnection, Edge>();
            
            graphViewChanged += OnGraphViewChanged;
            Undo.undoRedoPerformed += ReloadView;
        }

        public void PopulateView(SerializedObject serializedObject, GameEventEditorWindow window)
        {
            _gameEvent = (GameEvent)serializedObject.targetObject;
            _serializedObject = serializedObject;
            _window = window;

            if (_gameEvent == null) return;
            
            if (_gameEvent.rootNode == null) 
            {
                _gameEvent.rootNode = _gameEvent.AddNode(typeof(RootNode)) as RootNode;
                EditorUtility.SetDirty(_gameEvent);
                AssetDatabase.SaveAssets();
            }

            ReloadView();
        }

        public void Dispose()
        {
            graphViewChanged -= OnGraphViewChanged;
            Undo.undoRedoPerformed -= ReloadView;
        }

        private void ReloadView()
        {
            _reloadView = false;
            ClearView();
            
            DrawNodes();
            DrawConnections();

            graphViewChanged += OnGraphViewChanged;
            _currentGameEventView = this;
        }

        public void ClearView()
        {
            graphViewChanged -= OnGraphViewChanged;
            RemoveNodes();
            RemoveConnections();
        }

        public void UpdateNodeStates() 
        {
            nodes.ForEach(n => 
            {
                GameEventNodeView view = n as GameEventNodeView;
                view.UpdateState();
            });
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> allPorts = new List<Port>();
            List<Port> ports = new List<Port>();

            foreach (var node in _graphNodes)
            {
                allPorts.AddRange(node.allPorts);
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

                List<GameEventNodeView> removedNodes = new List<GameEventNodeView>();
                removedNodes.AddRange(graphviewchange.elementsToRemove.OfType<GameEventNodeView>());
                
                if (removedNodes.Count > 0)
                {
                    for (int i = removedNodes.Count - 1; i >= 0; i--)
                    {
                        DeleteNode(removedNodes[i]);
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
                
                nodes.ForEach((n) =>
                {
                    GameEventNodeView view = n as GameEventNodeView;
                    view.SortChildren();
                });

                foreach (var editorNode in graphviewchange.movedElements.OfType<GameEventNodeView>())
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

            if (_reloadView)
            {
                ReloadView();
            }
            
            EditorUtility.SetDirty(_gameEvent);

            return graphviewchange;
        }

        private void CreateEdge(Edge edge)
        {
            // edge.output and edge.input are reversed from what I would expect
            GameEventNodeView parentNodeView = (GameEventNodeView)edge.output.node;
            GameEventNodeView childNodeView = (GameEventNodeView)edge.input.node;
            
            GameEventConnection connection =
                new GameEventConnection(parentNodeView.GetOutputPortIndex(edge.output), parentNodeView.Id, childNodeView.Id);
            
            parentNodeView.AddConnection(connection);

            _connectionDictionary.Add(edge, connection);
            _edgeDictionary.Add(connection, edge);
        }

        private void DrawNodes()
        {
            if (_gameEvent.nodes == null) return;
            foreach (var node in _gameEvent.nodes)
            {
                AddNodeToGraph(node);
            }
        }

        private void DrawConnections()
        {
            _gameEvent.nodes.ForEach(n =>
            {
                for (int i = n.connections.Count-1; i >= 0; i--)
                {
                    AddConnection(GetNode(n.Id), n.connections[i]);
                }
            });
        }

        private void AddNode(Type nodeType, Vector2 mousePosition)
        {
            Undo.RecordObject(_serializedObject.targetObject, "Add Node");
            
            var node = _gameEvent.AddNode(nodeType);
            node.SetPosition(new Rect(mousePosition, new Vector2()));

            _serializedObject.Update();

            AddNodeToGraph(node);
        }

        private void AddNodeToGraph(GameEventNode node)
        {
            GameEventNodeView nodeView = new GameEventNodeView(node);
            //editorNode.SetPosition(node.Position);
            nodeView.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Assets/Scripts/GameEventSystem/Editor/Graph/USS/Node.uss"));

            _graphNodes.Add(nodeView);
            nodeView.OnNodeSelected += NodeSelected;
            nodeView.OnNodeUnselected += NodeUnselected;
            _nodeDictionary.Add(node.Id, nodeView);

            AddElement(nodeView);
        }

        private void NodeSelected(GameEventNodeView obj)
        {
            OnNodeSelected?.Invoke(obj);
        }

        private void NodeUnselected(GameEventNodeView obj)
        {
            OnNodeUnselected?.Invoke(obj);
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
            _edgeDictionary.Clear();
        }

        private void DeleteNode(GameEventNodeView nodeView)
        {
            if (_gameEvent.rootNode == nodeView.Node)
            {
                _reloadView = true;
                return;
            }
            
            // Unity does not automatically remove connections that are not in it's default input/output ports
            _gameEvent.nodes.ForEach(n =>
            {
                for (int i = n.connections.Count-1; i >= 0; i--)
                {
                    if (n.connections[i].outputNodeId.Equals(nodeView.Id) || n.connections[i].inputNodeId.Equals(nodeView.Id))
                    {
                        if (_edgeDictionary.ContainsKey(n.connections[i]))
                        {
                            RemoveElement(_edgeDictionary[n.connections[i]]);
                        }
                    }
                }
            });
            
            _gameEvent.RemoveNode(nodeView.Node);
            _graphNodes.Remove(nodeView);
            _nodeDictionary.Remove(nodeView.Id);
            _serializedObject.Update();
        }

        private void AddConnection(GameEventNodeView inputNodeView, GameEventConnection connection)
        {
            GameEventNodeView outputNodeView = GetNode(connection.outputNodeId);

            if (inputNodeView == null || outputNodeView == null) return;

            try
            {
                Port inputPort = inputNodeView.outputPorts[connection.portId];
                Port outputPort = outputNodeView.inputPort;

                Edge edge = inputPort.ConnectTo(outputPort);


                AddElement(edge);
                _connectionDictionary.Add(edge, connection);
                _edgeDictionary.Add(connection, edge);
            }
            catch (Exception e)
            {
                Debug.LogError("Connection no longer valid");
                
                inputNodeView.RemoveConnection(connection);
            }
        }

        private void RemoveConnection(Edge edge)
        {
            if (!_connectionDictionary.ContainsKey(edge)) return;

            GameEventConnection connection = _connectionDictionary[edge];
            GetNode(connection.inputNodeId)?.RemoveConnection(connection);
            
            _connectionDictionary.Remove(edge);
            _edgeDictionary.Remove(connection);
        }

        private GameEventNodeView GetNode(string nodeId)
        {
            GameEventNodeView nodeView = null;
            _nodeDictionary.TryGetValue(nodeId, out nodeView);

            return nodeView;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (evt.target.GetType() == typeof(Edge))
            {
                base.BuildContextualMenu(evt);
                return;
            }
            
            if (evt.target.GetType() != typeof(GameEventView))
            {
                return;
            }

            #region Add Trigger Search Menu
            evt.menu.AppendAction("Add Trigger Node", eventAction =>
            {
                var provider = new TriggerNodeSearchProvider((type) =>
                {
                    var windowMousePosition =
                        this.ChangeCoordinatesTo(this, eventAction.eventInfo.mousePosition - _window.position.position);
                    var mousePosition = contentViewContainer.WorldToLocal(windowMousePosition);
                    
                    AddNode(type, mousePosition);
                });
                SearchWindow.Open(new SearchWindowContext(eventAction.eventInfo.mousePosition),
                    provider);
            });
            #endregion

            #region Add Condition Search Menu
            evt.menu.AppendAction("Add Condition Node", AddNodeToBuildMenu(typeof(ConditionNode)));
            evt.menu.AppendAction("OR Node", AddNodeToBuildMenu(typeof(ORNode))); //TODO: Add decorator tab
            #endregion
            
            #region Add Effect Search Menu
            evt.menu.AppendAction("Add Effect Node", eventAction =>
            {
                var provider = new EffectNodeSearchProvider((type) =>
                {
                    var windowMousePosition =
                        this.ChangeCoordinatesTo(this, eventAction.eventInfo.mousePosition - _window.position.position);
                    var mousePosition = contentViewContainer.WorldToLocal(windowMousePosition);


                    AddNode(type, mousePosition);
                });
                SearchWindow.Open(new SearchWindowContext(eventAction.eventInfo.mousePosition),
                    provider);
            });
            #endregion
        }

        private Action<DropdownMenuAction> AddNodeToBuildMenu(Type type)
        {
            return eventAction =>
            {
                var windowMousePosition =
                    this.ChangeCoordinatesTo(this, eventAction.eventInfo.mousePosition - _window.position.position);
                var mousePosition = contentViewContainer.WorldToLocal(windowMousePosition);
                AddNode(type, mousePosition);
            };
        }
    }
}
