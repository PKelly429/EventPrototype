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
        private Label _blackboardTitle;

        private GameEvent _currentGameEvent;
        private AssetBlackboard _blackboard;
        private VisualElement _blackboardContent;

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

            _blackboardTitle = contentContainer.Q<Label>("blackboard-name");
            
            _blackboardContent = contentContainer.Q<ScrollView>("blackboard-params");
        }
        
        public void SetGameEvent(GameEvent gameEvent)
        {
            _currentGameEvent = gameEvent;
            if (_currentGameEvent == null)
            {
                _blackboardField.Unbind();
                _blackboardField.SetEnabled(false);
                _useLocalBlackboard.SetEnabled(false);
                _blackboardField.value = null;
                SetBlackboard(null);
                return;
            }
            
            _blackboardField.SetEnabled(true);
            _useLocalBlackboard.SetEnabled(true);
            _blackboardField.bindingPath ="blackboard";
            _blackboardField.Bind(new SerializedObject(_currentGameEvent));
            
            SetBlackboard(gameEvent.blackboard);
        }

        public override void HandleEvent(EventBase evt)
        {
            base.HandleEvent(evt);
            
            if (_currentGameEvent == null) return;
            if (evt.target == _blackboardField)
            {
                if (_blackboard != _currentGameEvent.blackboard)
                {
                    SetBlackboard(_currentGameEvent.blackboard);
                    _currentGameEvent.Bind();
                }
            }
        }

        public void SetBlackboard(AssetBlackboard blackboard)
        {
            _blackboard = blackboard;
            _blackboardContent.Clear();

            if (_blackboard == null)
            {
                _blackboardTitle.text = "none";
                return;
            }

            _blackboardTitle.text = blackboard.name;
            foreach (var property in _blackboard.definedVariables)
            {
                AddBlackboardProperty(property);
            }
        }
        
        public void AddBlackboardProperty(VariableDefinition property)
        {
            if (property == null) return;

            BlackboardFieldView propertyView = new BlackboardFieldView();
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
            _currentGameEvent.Bind();
        }
        
        private void AddButtonOnclicked()
        {
            if (_blackboard == null) return;
            
            var provider = new VariableDefinitionSearchProvider((type) =>
            {
                var newProperty = _blackboard.AddVariable(type);
                AddBlackboardProperty(newProperty);
            });
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
                provider);
        }
    }
}
