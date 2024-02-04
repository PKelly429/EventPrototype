using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameEventSystem.Editor
{
    [CustomPropertyDrawer(typeof(VariableReference), true)]
    public class BlackboardParamPropertyDrawer : PropertyDrawer
    {
        private GameEventNode _node;
        private VariableReference _target;
        private SerializedProperty _serializedProperty;

        private PropertyField _refField;
        private PropertyField _propertyNameField;
        private PropertyField _valueField;
        private Label _targetField;
        private Label _infoField;

        private Button _connectButton;
        private Button _disconnectButton;
        
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            _serializedProperty = property;
            _node = property.serializedObject.targetObject as GameEventNode;
            _target = ((VariableReference)_node.GetType().GetField(property.name).GetValue(_node));

            VisualElement propertyContainer = new VisualElement();
            
            var repaintField = new PropertyField(property.FindPropertyRelative("name"));
            repaintField.RegisterValueChangeCallback(Repaint);
            repaintField.style.display = DisplayStyle.None;
            propertyContainer.Add(repaintField);

            propertyContainer.AddToClassList("BBParam");
            
            VisualElement propertyLabel = new VisualElement();
            
            Label label = new Label($"BBParam: {GetDisplayString(_serializedProperty.name)}");
            propertyLabel.Add(label);

            VisualElement valueContainer = new VisualElement();
            
            valueContainer.AddToClassList("BBContents");
            
            propertyContainer.Add(propertyLabel);
            propertyContainer.Add(valueContainer);
            
            VisualElement topRow = new VisualElement();
            VisualElement bottomRow = new VisualElement();
            
            valueContainer.Add(topRow);
            valueContainer.Add(bottomRow);
            
            topRow.style.flexDirection = FlexDirection.Row;
            topRow.style.justifyContent = Justify.FlexEnd;
            bottomRow.style.flexDirection = FlexDirection.Row;

            propertyContainer.Add(_refField);

            _valueField = new PropertyField();
            _valueField.BindProperty(property.FindPropertyRelative("_localValue"));
            _valueField.label = string.Empty;
            _valueField.style.flexGrow = 1;
            bottomRow.Add(_valueField);

            _targetField = new Label();
            _targetField.style.flexGrow = 1;
            bottomRow.Add(_targetField);

            _infoField = new Label("Value:");
            _infoField.style.flexGrow = 1;
            topRow.Add(_infoField);

            _connectButton = new Button(() =>
            {
                ShowEntitySearchWindow.Open(_node.blackboard, (VariableReference)_node.GetType().GetField(property.name).GetValue(_node));
            });
            _connectButton.text = $"+";
            _connectButton.AddToClassList("connectBBParam");
            
            topRow.Add(_connectButton);
            
            _disconnectButton = new Button(() =>
            {
               ((VariableReference)_node.GetType().GetField(property.name).GetValue(_node)).RemoveRef();
            });
            _disconnectButton.text = $"+";
            _disconnectButton.AddToClassList("disconnectBBParam");
            
            topRow.Add(_disconnectButton);
            
            Repaint();
            return propertyContainer;
        }

        private void Repaint(SerializedPropertyChangeEvent evt)
        {
            Repaint();
        }

        void Repaint()
        {
            bool isBound = _target.HasRef;

            if (isBound)
            {
                _targetField.text = $"> {_target.name}";
            }

            _infoField.text = isBound ? $"{_node.blackboard.name}" : "Value:";
            
            _valueField.style.display =
                isBound ? DisplayStyle.None : DisplayStyle.Flex;
            _targetField.style.display =
                isBound ? DisplayStyle.Flex : DisplayStyle.None;
            
            _disconnectButton.style.display =
                isBound ? DisplayStyle.Flex : DisplayStyle.None;

        }
        
        private string GetDisplayString(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName)) return string.Empty;
			
            string str = fieldName.Replace("_", string.Empty);
            return $"{str[0].ToString().ToUpper()}{str.Substring(1)}";
        }
    }
}
