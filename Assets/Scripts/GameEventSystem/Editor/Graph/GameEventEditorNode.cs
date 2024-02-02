using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameEventSystem.Editor
{
    public class GameEventEditorNode : Node
    {
        private GameEventNode _node;

        public GameEventNode Node => _node;

        public string Id => Node.Id;

        public List<Port> Ports = new List<Port>();
        public Port InputPort => _inputPort;
        public Port OutputPort => _outputPort;
        
        private Port _inputPort;
        private Port _outputPort;
        
        public GameEventEditorNode(GameEventNode node) : base("Assets/Scripts/GameEventSystem/Editor/Graph/USS/NodeView.uxml")
        {
            _node = node;
            
            SetupClassLists();

            Type typeInfo = node.GetType();

            NodeInfoAttribute info = typeInfo.GetCustomAttribute<NodeInfoAttribute>() ?? new NodeInfoAttribute(typeInfo.Name);

            title = info.NodeTitle;

            string[] menuName = info.GetSplittedMenuName();
            foreach (var menu in menuName)
            {
                this.AddToClassList(menu.ToLower().Replace(' ', '-'));
            }

            this.name = typeInfo.Name;

            if (info.HasFlowInput)
            {
                CreateFlowInputPort();
            }
            if (info.HasFlowOutput)
            {
                CreateFlowOutputPort();
            }
        }

        private void SetupClassLists()
        {
            AddToClassList("game-event-node");

            var nodeType = _node.GetType();
            
            if (typeof(TriggerNode).IsAssignableFrom(nodeType))
            {
                AddToClassList("trigger-node");
            }
            else if (typeof(ConditionNode).IsAssignableFrom(nodeType))
            {
                AddToClassList("condition-node");
            }
            else if (typeof(EffectNode).IsAssignableFrom(nodeType))
            {
                AddToClassList("effect-node");
            }
        }

        private void CreateFlowInputPort()
        {
            _inputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Multi, typeof(PortTypes.FlowPort));
            _inputPort.portName = string.Empty;
            _inputPort.style.flexDirection = FlexDirection.Column;
            _inputPort.tooltip = "Flow input";
            Ports.Add(_inputPort);
            inputContainer.Add(_inputPort);
        }
        
        private void CreateFlowOutputPort()
        {
            _outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(PortTypes.FlowPort));
            _outputPort.portName = string.Empty;
            _outputPort.style.flexDirection = FlexDirection.ColumnReverse;
            _outputPort.tooltip = "Flow output";
            Ports.Add(_outputPort);
            outputContainer.Add(_outputPort);
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
    }
}
