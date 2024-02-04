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
        
        [NonSerialized] public State state = State.Idle;
        [HideInInspector] public AssetBlackboard blackboard;
        
        [NonSerialized] public List<GameEventNode> children = new List<GameEventNode>();
        [HideInInspector] public List<GameEventConnection> connections = new List<GameEventConnection>();

        public string Id => guid;
        public Rect Position => _position;

        public void Setup(GameEvent parent)
        {
            foreach (var connection in connections)
            {
                if (connection.portId == 0)
                {
                    children.Add(parent.GetNode(connection.outputNodeId));   
                }
            }

            SortChildren();
            
            OnSetup();
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
            if (state == State.Idle) 
            {
                OnStart();
            }

            state = OnUpdate();
            
            if (state != State.Running) 
            {
                OnStop();
            }
            
            return state;
        }

        protected virtual void OnStart() { }

        protected virtual void OnStop() { }
        
        protected abstract State OnUpdate();

        #region Graph
        public void DrawInspector(VisualElement contentContainer)
        {
            SerializedObject obj = new SerializedObject(this);
            
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

            var bbParams = GetType().GetFields().Where(fieldInfo => typeof(VariableReference).IsAssignableFrom(fieldInfo.FieldType));
            foreach (var fieldInfo in bbParams)
            {
                VariableReference param = fieldInfo.GetValue(this) as VariableReference;
                param?.ResolveRef(this.blackboard);
            }
        }


        
        public virtual void DrawContent(VisualElement contentContainer)
        {
#if UNITY_EDITOR
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
