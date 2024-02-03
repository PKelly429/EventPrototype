using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
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
        
        [HideInInspector] public State state = State.Idle;
        [SerializeField] [HideInInspector] private string guid;
        
        [HideInInspector] public List<GameEventNode> Outputs = new List<GameEventNode>();
        [HideInInspector] [SerializeField] private Rect _position;

        public string Id => guid;
        public Rect Position => _position;

        public virtual void Setup()
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
        
        public void AddOutput(GameEventNode node)
        {
            Outputs.Add(node);
            EditorUtility.SetDirty(this);
        }

        public void RemoveOutput(GameEventNode node)
        {
            Outputs.Remove(node);
            EditorUtility.SetDirty(this);
        }

        public void GenerateGUID()
        {
            guid = GUID.Generate().ToString();
        }
        #endregion
    }
}
