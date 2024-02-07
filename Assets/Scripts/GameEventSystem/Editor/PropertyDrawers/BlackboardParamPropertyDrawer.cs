using System;
using System.Collections;
using System.Collections.Generic;
using Codice.CM.Common.Tree.Partial;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameEventSystem.Editor
{
    [CustomPropertyDrawer(typeof(VariableReference), true)]
    public class BlackboardParamPropertyDrawer : PropertyDrawer
    {
        // CANT SAVE ANY REFERENCES AS THIS CLASS GETS REUSED BY EVERY DRAWER

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty _serializedProperty = property;
            GameEventNode node = property.serializedObject.targetObject as GameEventNode;

            VariableReference target = (VariableReference)EditorUtils.GetTargetObjectOfProperty(property);

            VisualElement propertyContainer = new VisualElement();

            propertyContainer.AddToClassList("BBParam");
            
            VisualElement propertyLabel = new VisualElement();
            
            Label label = new Label($"BBParam: {EditorUtils.GetDisplayString(_serializedProperty.name)}");
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
            

            PropertyField valueField = new PropertyField();
            valueField.BindProperty(property.FindPropertyRelative("_localValue"));
            valueField.label = string.Empty;
            valueField.style.flexGrow = 1;
            bottomRow.Add(valueField);

            Label targetField = new Label();
            targetField.style.flexGrow = 1;
            bottomRow.Add(targetField);

            Label infoField = new Label("Value:");
            infoField.style.flexGrow = 1;
            topRow.Add(infoField);
            
            Button connectButton = new Button(() =>
            {
                if (node == null || node.blackboard == null)
                {
                    Debug.Log("BBProperty callback has no blackboard reference");
                    return;
                }
                ShowEntitySearchWindow.Open(node.blackboard, target);
            });

            connectButton.text = $"+";
            connectButton.AddToClassList("connectBBParam");
            
            topRow.Add(connectButton);
            
            Button disconnectButton = new Button(() =>
            {
                target.RemoveRef();
            });
            disconnectButton.text = $"+";
            disconnectButton.AddToClassList("disconnectBBParam");
            
            topRow.Add(disconnectButton);
            
            var repaintField = new PropertyField(property.FindPropertyRelative("name"));
            repaintField.RegisterValueChangeCallback(delegate(SerializedPropertyChangeEvent evt)
            {
                Redraw(node, connectButton, target, targetField, infoField, valueField, disconnectButton);
            });
            repaintField.style.display = DisplayStyle.None;
            propertyContainer.Add(repaintField);

            Redraw(node, connectButton, target, targetField, infoField, valueField, disconnectButton);
            
            return propertyContainer;
        }

        private static void Redraw(GameEventNode node, Button connectButton, VariableReference target, Label targetField,
            Label infoField, PropertyField valueField, Button disconnectButton)
        {
            if (node == null) return;

            connectButton.SetEnabled(node.blackboard != null);

            bool isBound = target.HasRef && node.blackboard != null;

            if (isBound)
            {
                targetField.text = $"> {target.name}";
            }

            infoField.text = isBound ? $"{node.blackboard.name}" : "Value:";

            valueField.style.display =
                isBound ? DisplayStyle.None : DisplayStyle.Flex;
            targetField.style.display =
                isBound ? DisplayStyle.Flex : DisplayStyle.None;

            disconnectButton.style.display =
                isBound ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
