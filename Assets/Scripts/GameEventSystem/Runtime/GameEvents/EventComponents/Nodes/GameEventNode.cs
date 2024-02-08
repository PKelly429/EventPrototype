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
        
        public State state { get; private set; } = State.Idle;
        [NonSerialized] protected GameEvent runtimeGameEvent;
        [NonSerialized] private bool _isSubscribedToEventRunner;
        
        public virtual bool IsRootNode => false;
        public virtual bool IsTriggerNode => false;
        public virtual bool IsConditionNode => false;

        public virtual bool ShouldUpdateWhenRunning => true;
        
        public string Id => guid;
        public Rect Position => _position;

        public void SetState(State newState)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            
            if (state == newState) return;

            state = newState;

            if (!ShouldUpdateWhenRunning) return;
            
            if (state == State.Running)
            {
                SubscribeToEventRunner();
            }
            else
            {
                UnsubscribeToEventRunner();
            }
        }

        public void Setup(GameEvent parent)
        {
            try
            {
                runtimeGameEvent = parent;
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

        public State Execute()
        {
            if (state is State.Success or State.Failure)
            {
                return state;
            }
            
            if (state != State.Running) 
            {
                runtimeGameEvent.SetNodeActive(this);
                OnStart();
            }

            SetState(OnUpdate());

            if (state is State.Success or State.Failure) 
            {
                OnStop();
                runtimeGameEvent.SetNodeComplete(this);
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
        
        /// <summary>
        /// Called when event is loaded
        /// </summary>
        protected virtual void OnSetup()
        {
            
        }

        /// <summary>
        /// Called when node is executed and is not already running
        /// </summary>
        protected virtual void OnStart() { }

        /// <summary>
        /// Called when node completes with either Success or Failure
        /// </summary>
        protected virtual void OnStop()
        {
            if (state == State.Running) return;

            foreach (var child in children)
            {
                child.Execute();
            }
        }
        
        protected abstract State OnUpdate();
        
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

        #region Graph
        public virtual void PerformTestGraphFunction() { }
        
        public void DrawInspector(VisualElement contentContainer)
        {
#if UNITY_EDITOR
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
                //if (field.GetCustomAttribute(typeof(DisplayFieldAttribute)) != null) continue;

                var propertyField = new PropertyField();
                propertyField.bindingPath = field.Name;
                propertyField.Bind(obj);
                contentContainer.Add(propertyField);
            }
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
        
        public string GetDebugName()
        {
            return $"{runtimeGameEvent.name}/{name}";
        }

        public void GenerateGUID()
        {
            guid = GUID.Generate().ToString();
        }
        #endregion
    }
}
