using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace GameEventSystem
{
    [Serializable]
    public abstract class GameEventNode : ScriptableObject
    {
        public enum State 
        {
            Idle,
            Running,
            Failure,
            Success
        }
        [HideInInspector] [SerializeField] private string guid;
        [HideInInspector] [SerializeField] private Rect _position;
        
        [HideInInspector] public AssetBlackboard blackboard;
        
        [NonSerialized] public List<GameEventNode> children = new List<GameEventNode>();
        [NonSerialized] public List<GameEventNode> conditionNodes = new List<GameEventNode>();
        [HideInInspector] public List<GameEventConnection> connections = new List<GameEventConnection>();
        
        [NonSerialized] public State state = State.Idle;
        [NonSerialized] protected GameEvent _runtimeGameEvent;
        [NonSerialized] private bool _isSubscribedToEventRunner;
        
        public virtual bool IsRootNode => false;
        public virtual bool IsTriggerNode => false;
        public virtual bool IsConditionNode => false;

        public string Id => guid;
        public Rect Position => _position;

        public void Setup(GameEvent parent)
        {
            try
            {
                _runtimeGameEvent = parent;
                foreach (var connection in connections)
                {
                    var nodeToAdd = parent.GetNode(connection.outputNodeId);
                    if (nodeToAdd.IsConditionNode)
                    {
                        conditionNodes.Add(nodeToAdd);
                    }
                    else
                    {
                        children.Add(nodeToAdd);
                    }
                }

                SortChildren();

                OnSetup();
            }
            catch (Exception e)
            {
                #if DEBUG
                Debug.LogException(e);
                Debug.Log($"Caught exception while setting up event: {name}");
                #endif
            }
        }

        public virtual bool CheckConditions()
        {
            foreach (var conditionNode in conditionNodes)
            {
                if (!conditionNode.CheckConditions())
                {
                    state = State.Failure;
                    return false;
                }
            }

            return true;
        }
        
        public void SortChildren() 
        {
            children.Sort(SortByHorizontalPosition);
        }
        
        private int SortByHorizontalPosition(GameEventNode left, GameEventNode right) 
        {
            return left.Position.x < right.Position.x ? -1 : 1;
        }
        
        public virtual void OnSetup()
        {
            
        }

        public virtual State Execute()
        {
            if (state != State.Running) 
            {
                OnStart();
            }

            state = OnUpdate();
            
            if (state == State.Running)
            {
                SubscribeToEventRunner();
            }
            else
            {
                UnsubscribeToEventRunner();
            }
            
            if (state != State.Running) 
            {
                OnStop();
            }
            
            return state;
        }

        protected void SubscribeToEventRunner()
        {
            if (_isSubscribedToEventRunner) return;
            
            _isSubscribedToEventRunner = true;
            EventManager.RegisterToEventRunner(this);
        }
        
        protected void UnsubscribeToEventRunner()
        {
            if (!_isSubscribedToEventRunner) return;
            
            _isSubscribedToEventRunner = false;
            EventManager.DeregisterToEventRunner(this);
        }

        protected virtual void OnStart() { }

        protected virtual void OnStop()
        {
            if (state != State.Success) return;

            foreach (var child in children)
            {
                child.Execute();
            }
        }
        
        protected abstract State OnUpdate();

        #region Graph
        public virtual void PerformTestGraphFunction() { }
        public void DrawInspector(VisualElement contentContainer)
        {
            SerializedObject obj = new SerializedObject(this);
            
            NodeInfoAttribute info = GetType().GetCustomAttribute<NodeInfoAttribute>() ?? new NodeInfoAttribute(name);
            
            Label nodeHeader = new Label(info.NodeTitle);
            nodeHeader.AddToClassList("node-header");
            contentContainer.Add(nodeHeader);
            
            var fields = GetType().GetFields();
            foreach (var field in fields)
            {
                if (field.IsNotSerialized) continue;
                if (field.GetCustomAttribute(typeof(HideInInspector)) != null) continue;
                if (field.GetCustomAttribute(typeof(DisplayFieldAttribute)) != null) continue;

                var propertyField = new PropertyField();
                propertyField.bindingPath = field.Name;
                propertyField.Bind(obj);
                contentContainer.Add(propertyField);
            }
        }

        public void BindBlackboard(AssetBlackboard blackboard)
        {
            this.blackboard = blackboard;
            
            var bindableFields = GetType().GetFields().Where(fieldInfo => typeof(IBindable).IsAssignableFrom(fieldInfo.FieldType));
            foreach (var fieldInfo in bindableFields)
            {
                IBindable field = fieldInfo.GetValue(this) as IBindable;
                field?.Bind(this.blackboard);
            }

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);

            RequestRedraw();
#endif
        }

#if UNITY_EDITOR
        private VisualElement _contentContainer;

        public void RequestRedraw()
        {
            // Dedraw the node contents when the blackboard changes - this fixes some weird problem with the
            // UI callbacks becoming stale when a new blackboard is made
            if (_contentContainer != null)
            {
                _contentContainer.Clear();
                DrawContent(_contentContainer);
            }
        }

        public string GetDebugName()
        {
            return $"{_runtimeGameEvent.name}/{name}";
        }
#endif
        public virtual void DrawContent(VisualElement contentContainer)
        {
#if UNITY_EDITOR
            _contentContainer = contentContainer;
            var fieldsToDisplay = GetType().GetFields()
                .Where(fieldInfo => fieldInfo.IsDefined(typeof(DisplayFieldAttribute), false));

            foreach (var fieldInfo in fieldsToDisplay)
            {
                VisualElement fieldContainer = new VisualElement();
                fieldContainer.style.marginTop = 5;
                
                PropertyField propertyField = new PropertyField(); 
                propertyField.bindingPath = fieldInfo.Name;
                propertyField.BindProperty(new SerializedObject(this));
                fieldContainer.Add(propertyField);
                contentContainer.Add(fieldContainer);
            }
#endif
        }

        public virtual GameEventNode Clone() 
        {
            return Instantiate(this);
        }
        public void SetPosition(Rect position)
        {
            _position = position;
            EditorUtility.SetDirty(this);
        }
        
        public void AddConnection(GameEventConnection connection)
        {
            connections.Add(connection);
            EditorUtility.SetDirty(this);
        }

        public void RemoveConnection(GameEventConnection connection)
        {
            connections.Remove(connection);
            EditorUtility.SetDirty(this);
        }

        public void GenerateGUID()
        {
            guid = GUID.Generate().ToString();
        }
        #endregion
    }
}
