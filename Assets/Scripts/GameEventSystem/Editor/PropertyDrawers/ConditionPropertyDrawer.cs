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
    [CustomPropertyDrawer(typeof(Condition), true)]
    public class ConditionPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = new VisualElement();
            
            
            VisualElement buttonContainer = new VisualElement();
            buttonContainer.style.flexDirection = FlexDirection.RowReverse;

            var clearButton = new Button(() =>
            {
                property.FindPropertyRelative("condition").managedReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
                
                (property.serializedObject.targetObject as GameEventNode)?.RequestRedraw();
            });
            clearButton.AddToClassList("remove-condition-button");
            clearButton.text = "+";
            buttonContainer.Add(clearButton);
            
            var setConditionButton = new Button(() =>
            {
                var provider = new ConditionSearchProvider((type) =>
                {
                    property.FindPropertyRelative("condition").managedReferenceValue = Activator.CreateInstance(type);
                    property.serializedObject.ApplyModifiedProperties();
                    
                    (property.serializedObject.targetObject as GameEventNode)?.RequestRedraw();
                });
                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
                    provider);
            });
            setConditionButton.AddToClassList("set-condition-button");
            setConditionButton.text = "+";
            buttonContainer.Add(setConditionButton);

            if (property.FindPropertyRelative("condition").managedReferenceValue == null)
            {
                AddLabel(buttonContainer, "Not Set");
            }
            else
            {
                var conditionType = property.FindPropertyRelative("condition").managedReferenceValue.GetType();
                var menuAttribute = conditionType.GetCustomAttribute<AddTypeMenuAttribute>();
                string name = menuAttribute == null ? conditionType.Name : menuAttribute.MenuName;
                AddLabel(buttonContainer, name);
            }


            container.Add(buttonContainer);
            PropertyField conditionField = new PropertyField(property.FindPropertyRelative("condition"));
            // if (property.FindPropertyRelative("condition").managedReferenceValue != null)
            // {
            //     conditionField.label = $"{property.FindPropertyRelative("condition").managedReferenceValue.GetType().Name}";
            // }

            container.Add(conditionField);

            return container;
        }

        private void AddLabel(VisualElement container, string text)
        {
            Label label = new Label(text);
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            label.style.fontSize = 14;
            label.style.flexGrow = 1;
            label.style.marginLeft = 4;
            container.Add(label);
        }
        
    }
}