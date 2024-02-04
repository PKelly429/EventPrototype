using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameEventSystem.Editor
{
    public class BlackboardView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<BlackboardView, VisualElement.UxmlTraits> { }
        
        private ObjectField _blackboardField;
        private Button _useLocalBlackboard;
        private Button _addButton;

        private GameEvent _currentGameEvent;
        private AssetBlackboard _blackboard;
        private VisualElement _blackboardContent;
        
        private Dictionary<Type, Type> blackboardFieldTypes = new Dictionary<Type, Type>();

        public BlackboardView()
        {
        }

        public void SetupBlackboard()
        {
            _blackboardField = contentContainer.Q<ObjectField>("blackboard-field");
            _blackboardField.objectType = typeof(AssetBlackboard);
            
            _useLocalBlackboard = contentContainer.Q<Button>("use-local-blackboard");
            _addButton = contentContainer.Q<Button>("addButton");
            _useLocalBlackboard.clicked += UseLocalBlackboardOnclicked;
            _addButton.clicked += AddButtonOnclicked;

            _blackboardContent = contentContainer.Q<ScrollView>("blackboard-params");

            InitBlackboardFields();
        }

        private void InitBlackboardFields()
        {
            if (blackboardFieldTypes == null)
            {
                blackboardFieldTypes = new Dictionary<Type, Type>();
            }
            blackboardFieldTypes.Clear();
            
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(typeof(BlackboardFieldView)) == true && type.IsAbstract == false)
                    {
                        BlackboardPropertyTypeAttribute fieldInfo = type.GetCustomAttribute<BlackboardPropertyTypeAttribute>();
                        if (fieldInfo == null) continue;
                        blackboardFieldTypes.Add(fieldInfo.type, type);
                    }
                }
            }
        }

        public void SetGameEvent(GameEvent gameEvent)
        {
            _currentGameEvent = gameEvent;
            if (_currentGameEvent == null || _currentGameEvent.Blackboard == null)
            {
                _blackboardField.Unbind();
                return;
            }
            _blackboardField.bindingPath ="Blackboard";
            _blackboardField.Bind(new SerializedObject(_currentGameEvent));
        }

        public override void HandleEvent(EventBase evt)
        {
            base.HandleEvent(evt);

            if (_currentGameEvent == null) return;
            if (evt.target == _blackboardField)
            {
                // event for when the blackboard is changed
                if (evt.eventTypeId != 37) return;
                SetBlackboard(_currentGameEvent.Blackboard);
            }
        }

        public void SetBlackboard(AssetBlackboard blackboard)
        {
            _blackboard = blackboard;
            
            _blackboardContent.Clear();
            foreach (var property in _blackboard.definedVariables)
            {
                AddBlackboardProperty(property);
            }
        }
        
        public void AddBlackboardProperty(VariableDefinition property)
        {
            if (property == null) return;
            if (!blackboardFieldTypes.ContainsKey(property.GetType())) return;
            Type fieldType = blackboardFieldTypes[property.GetType()];
            BlackboardFieldView propertyView = Activator.CreateInstance(fieldType) as BlackboardFieldView;
            propertyView.CreateView(_blackboard, property);
            propertyView.editTextRequested += EditTextRequested;
            propertyView.onRemoveBlackboardProperty += OnRemoveBlackboardProperty;
            _blackboardContent.Add(propertyView);
        }
        
        void OnRemoveBlackboardProperty(BlackboardFieldView field)
        {
            if (_blackboard == null) return;
            Undo.RecordObject(_blackboard, "Remove Blackboard Property");
        
            _blackboard.RemoveVariable(field.property);
            _blackboardContent.Remove(field);

            EditorUtility.SetDirty(_blackboard);
        }
        
        public void EditTextRequested(VisualElement visualElement, string newText)
        {
            if (_blackboard == null) return;
            
            var field = (GameEventBlackboardField)visualElement;
            var property = (VariableDefinition)field.userData;

            if (!string.IsNullOrEmpty(newText) && newText != property.Name)
            {
                Undo.RecordObject(_currentGameEvent, "Edit Blackboard Text");
            
                int count = 0;
                string propertyName = newText;
                foreach (var boardProperty in _blackboard.definedVariables)
                {
                    if (boardProperty.Name == propertyName) count++;
                }
                if (count > 0) propertyName += $"({count})";
            
                property.Name = propertyName;
                field.text = property.Name;
                 
                EditorUtility.SetDirty(_blackboard);
            }
        }
        
        private void UseLocalBlackboardOnclicked()
        {
            if (_currentGameEvent == null) return;
            SetBlackboard(_currentGameEvent.CreateLocalBlackboard());
        }
        
        private void AddButtonOnclicked()
        {
            if (_currentGameEvent == null) return;
            
            var provider = new VariableDefinitionSearchProvider((type) =>
            {
                var newProperty = _currentGameEvent.AddVariable(type);
                AddBlackboardProperty(newProperty);
            });
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
                provider);
        }
    }
}
