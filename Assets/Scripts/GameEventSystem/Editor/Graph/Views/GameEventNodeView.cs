using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameEventSystem.Editor
{
    public class GameEventNodeView : Node
    {
        public Action<GameEventNodeView> OnNodeSelected;
        public Action<GameEventNodeView> OnNodeUnselected;
        
        private GameEventNode _node;
        public GameEventNode Node => _node;

        public string Id => Node.Id;

        public List<Port> allPorts = new List<Port>();
        public Port inputPort;
        public List<Port> outputPorts = new List<Port>();

        private VisualElement conditionInputContainer;
        private VisualElement conditionOutputContainer;

        public int GetOutputPortIndex(Port port)
        {
            return outputPorts.IndexOf(port);
        }
        
        public GameEventNodeView(GameEventNode node) : base("Assets/Scripts/GameEventSystem/Editor/Graph/USS/NodeView.uxml")
        {
            _node = node;

            if (_node.IsRootNode)
            {
                this.capabilities &= ~Capabilities.Copiable;
                this.capabilities &= ~Capabilities.Selectable;
                this.capabilities &= ~Capabilities.Movable;
                this.capabilities &= ~Capabilities.Deletable;
            }

            viewDataKey = _node.Id;
            style.left = node.Position.x;
            style.top = node.Position.y;

            SetupClassLists();

            Type typeInfo = node.GetType();

            NodeInfoAttribute info = typeInfo.GetCustomAttribute<NodeInfoAttribute>() ?? new NodeInfoAttribute(typeInfo.Name);
            NodeDescriptionAttribute description = typeInfo.GetCustomAttribute<NodeDescriptionAttribute>();

            
            conditionInputContainer = this.Q<VisualElement>("condition-input");
            conditionOutputContainer = this.Q<VisualElement>("condition-output");
            
            title = info.NodeTitle;
            Label descriptionLabel = this.Q<Label>("description-label");
            descriptionLabel.text = description == null ? string.Empty : description.NodeDescription;
            if (description == null) descriptionLabel.visible = false;
            
            Button scriptButton = this.Q<Button>("script-button");
            if(scriptButton != null) scriptButton.clicked += OpenScript;
            
            Button triggerButton = this.Q<Button>("node-function-button");
            if(triggerButton != null) triggerButton.clicked += TriggerEventFunction;

            string[] menuName = info.GetSplittedMenuName();
            foreach (var menu in menuName)
            {
                this.AddToClassList(menu.ToLower().Replace(' ', '-'));
            }

            this.name = typeInfo.Name;

            NodeConnectionInputAttribute inputInfo = typeInfo.GetCustomAttribute<NodeConnectionInputAttribute>();
            if (inputInfo == null)
            {
                inputInfo = new NodeConnectionInputAttribute(PortTypeDefinitions.PortTypes.Flow);
            }
            CreateInputPort(inputInfo);
            
            NodeConnectionOutputAttribute[] outputInfo = typeInfo.GetCustomAttributes<NodeConnectionOutputAttribute>().ToArray();
            if (outputInfo.Length == 0)
            {
                outputInfo = new NodeConnectionOutputAttribute[]
                {
                    new NodeConnectionOutputAttribute(PortTypeDefinitions.PortTypes.Flow)
                };
            }

            bool hasOutputPort = false;
            for (int i = 0; i < outputInfo.Length; i++)
            {
                CreateOutputPorts(outputInfo[i]);
            }

            AddNodeButton addNodeButtonInfo = typeInfo.GetCustomAttribute<AddNodeButton>();
            if (addNodeButtonInfo != null)
            {
                if (typeof(GameEventNode).IsAssignableFrom(addNodeButtonInfo.Type))
                {
                    AddToClassList("show-add-node-button");
                    Button button = this.Q<Button>("add-node-button");
                    button.clicked += () =>
                    {
                        GameEventView.AddChildButton(addNodeButtonInfo.Type, node.Position.position + new Vector2(150, 100));
                    };
                }
            }

            node.DrawContent(this.Q<VisualElement>("node-content"));
        }

        public override void OnSelected()
        {
            base.OnSelected();
            
            OnNodeSelected?.Invoke(this);
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            
            OnNodeUnselected?.Invoke(this);
        }

        public void OpenScript()
        {
            var scriptObj = MonoScript.FromScriptableObject(_node);
            AssetDatabase.OpenAsset(scriptObj);
        }

        public void TriggerEventFunction()
        {
            _node.PerformTestGraphFunction();
        }

        private void SetupClassLists()
        {
            AddToClassList("game-event-node");

            var nodeType = _node.GetType();
            
            //TODO: Make this an attribute
            if (typeof(TriggerNode).IsAssignableFrom(nodeType))
            {
                AddToClassList("trigger-node");
            }
            else if (typeof(ConditionNode).IsAssignableFrom(nodeType))
            {
                AddToClassList("condition-node");
            }
            else if (typeof(ORNode).IsAssignableFrom(nodeType))
            {
                AddToClassList("condition-node");
            }
            else if (typeof(ModalWindowChoiceNode).IsAssignableFrom(nodeType))
            {
                AddToClassList("choice-node");
            }
            else if (typeof(SelectNode).IsAssignableFrom(nodeType))
            {
                AddToClassList("select-node");
            }
            else if (typeof(EffectNode).IsAssignableFrom(nodeType))
            {
                AddToClassList("effect-node");
            }
            else if (typeof(RootNode).IsAssignableFrom(nodeType))
            {
                AddToClassList("root-node");
            }
            
            if (typeof(ModalWindowNode).IsAssignableFrom(nodeType))
            {
                AddToClassList("choice-window");
            }
        }
        

        private void CreateInputPort(NodeConnectionInputAttribute connectionInfo)
        {
            if (connectionInfo.Count < 1) return;
            if (connectionInfo.Count > 1)
            {
                Debug.LogError("Multiple input ports it not supported.");
            }
            
            inputPort = CreatePort(Direction.Input, connectionInfo.PortType);
            inputPort.portColor = PortTypeDefinitions.GetPortColour(connectionInfo.PortType);
            inputContainer.Add(inputPort);
            
            if (connectionInfo.PortType == PortTypeDefinitions.PortTypes.Condition)
            {
                inputPort.style.flexDirection = FlexDirection.Row;
                conditionInputContainer.Add(inputPort);
            }
            else
            {
                inputPort.style.flexDirection = FlexDirection.Column;
                inputContainer.Add(inputPort); 
            }
        }
        
        private void CreateOutputPorts(NodeConnectionOutputAttribute connectionInfo)
        {
            if (outputPorts == null) outputPorts = new List<Port>();
            for (int i = 0; i < connectionInfo.Count; i++)
            {
                Port newPort = CreatePort(Direction.Output, connectionInfo.PortType);
                newPort.portColor = PortTypeDefinitions.GetPortColour(connectionInfo.PortType);
                outputPorts.Add(newPort);

                if (connectionInfo.PortType == PortTypeDefinitions.PortTypes.Condition)
                {
                    newPort.style.flexDirection = FlexDirection.RowReverse;
                    conditionOutputContainer.Add(newPort);
                }
                else
                {
                    newPort.style.flexDirection = FlexDirection.ColumnReverse;
                    outputContainer.Add(newPort);   
                }
            }
        }

        private NodePort CreatePort(Direction direction, PortTypeDefinitions.PortTypes portType)
        {
            Orientation orientation = portType == PortTypeDefinitions.PortTypes.Condition ? Orientation.Horizontal : Orientation.Vertical;
            NodePort newPort = new NodePort(direction, Port.Capacity.Multi, portType, orientation);
            newPort.portName = string.Empty;
            newPort.AddToClassList($"{portType}");
            allPorts.Add(newPort);
            return newPort;
        }

        public void AddConnection(GameEventConnection connection)
        {
            _node.AddConnection(connection);
        }
        
        public void RemoveConnection(GameEventConnection connection)
        {
            _node.RemoveConnection(connection);
        }

        public void SavePosition()
        {
            _node.SetPosition(this.GetPosition());
        }
        
        public void SortChildren() 
        {
            _node.SortChildren();
        }
        

        public void UpdateState() 
        {
            RemoveFromClassList("idle");
            RemoveFromClassList("running");
            RemoveFromClassList("failure");
            RemoveFromClassList("success");

            if (Application.isPlaying) 
            {
                switch (_node.state) 
                {
                    case GameEventNode.State.Idle:
                        AddToClassList("idle");
                        break;
                    case GameEventNode.State.Running:
                        AddToClassList("running");
                        break;
                    case GameEventNode.State.Failure:
                        AddToClassList("failure");
                        break;
                    case GameEventNode.State.Success:
                        AddToClassList("success");
                        break;
                }
            }
        }
    }
}
