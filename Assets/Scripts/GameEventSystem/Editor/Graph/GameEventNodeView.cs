using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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

        public List<Port> Ports = new List<Port>();
        public Port InputPort => _inputPort;
        public Port OutputPort => _outputPort;
        
        private Port _inputPort;
        private Port _outputPort;
        
        public GameEventNodeView(GameEventNode node) : base("Assets/Scripts/GameEventSystem/Editor/Graph/USS/NodeView.uxml")
        {
            _node = node;

            viewDataKey = _node.Id;
            style.left = node.Position.x;
            style.top = node.Position.y;
            
            SetupClassLists();

            Type typeInfo = node.GetType();

            NodeInfoAttribute info = typeInfo.GetCustomAttribute<NodeInfoAttribute>() ?? new NodeInfoAttribute(typeInfo.Name);
            NodeDescriptionAttribute description = typeInfo.GetCustomAttribute<NodeDescriptionAttribute>();

            title = info.NodeTitle;
            Label descriptionLabel = this.Q<Label>("description-label");
            descriptionLabel.text = description == null ? string.Empty : description.NodeDescription;
            if (description == null) descriptionLabel.visible = false;

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
            _inputPort = new NodePort(Direction.Input, Port.Capacity.Multi);
            _inputPort.portName = string.Empty;
            _inputPort.style.flexDirection = FlexDirection.Column;
            _inputPort.tooltip = "Flow input";
            Ports.Add(_inputPort);
            inputContainer.Add(_inputPort);
        }
        
        private void CreateFlowOutputPort()
        {
            _outputPort = new NodePort(Direction.Output, Port.Capacity.Multi);
            _outputPort.portName = string.Empty;
            _outputPort.style.flexDirection = FlexDirection.ColumnReverse;
            _outputPort.tooltip = "Flow output";
            Ports.Add(_outputPort);
            outputContainer.Add(_outputPort);
        }

        public void AddOutput(GameEventNode node)
        {
            _node.AddOutput(node);
        }
        
        public void RemoveOutput(GameEventNode node)
        {
            _node.RemoveOutput(node);
        }

        public void SavePosition()
        {
            _node.SetPosition(this.GetPosition());
        }
        
        public void SortChildren() 
        {
            _node.Outputs.Sort(SortByHorizontalPosition);
        }
        
        private int SortByHorizontalPosition(GameEventNode left, GameEventNode right) 
        {
            return left.Position.x < right.Position.x ? -1 : 1;
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