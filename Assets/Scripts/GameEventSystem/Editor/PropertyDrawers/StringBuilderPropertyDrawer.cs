using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameEventSystem.Editor
{
    [CustomPropertyDrawer(typeof(StringParameterBuilder), true)]
    public class StringBuilderPropertyDrawer : PropertyDrawer
    {
        // Trying to add new fields in the callback does not work, so we make them all now and display later
        private const int maxToDisplay = 3;
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            StringParameterBuilder target = (StringParameterBuilder) EditorUtils.GetTargetObjectOfProperty(property);

            VisualElement container = new VisualElement();

            Label errorField = new Label("ERROR: String cannot be formatted.");
            errorField.style.display = DisplayStyle.None;

            PropertyField textField = new PropertyField(property.FindPropertyRelative("text"));
            textField.label = EditorUtils.GetDisplayString($"StringBuilder: {property.name}");
            textField.RegisterValueChangeCallback(delegate(SerializedPropertyChangeEvent evt)
            {
                target.HandleTextUpdated();
                ShowValidationMessage(target, errorField);
            });
            
            container.Add(textField);
            container.Add(errorField);
            
            VisualElement stringParamContainer = new VisualElement();
            container.Add(stringParamContainer);

            PropertyField[] paramFields = new PropertyField[maxToDisplay];
            for (int i = 0; i < maxToDisplay; i++)
            {
                paramFields[i] = new PropertyField();
                stringParamContainer.Add(paramFields[i]);
                paramFields[i].style.display = DisplayStyle.None;
            }
            
            PropertyField listenerField = new PropertyField(property.FindPropertyRelative("needsRepaint"));
            listenerField.style.display = DisplayStyle.None;
            listenerField.RegisterValueChangeCallback(delegate(SerializedPropertyChangeEvent evt)
            {
                if (property.FindPropertyRelative("needsRepaint").boolValue)
                {
                    for (int i = 0; i < maxToDisplay; i++)
                    {
                        if (target.parameters == null) break;
                        if (i < target.parameters.Length)
                        {
                            paramFields[i]
                                .BindProperty(property.FindPropertyRelative("parameters").GetArrayElementAtIndex(i));
                            paramFields[i].style.display = DisplayStyle.Flex;
                        }
                        else
                        {
                            paramFields[i].Unbind();
                            paramFields[i].style.display = DisplayStyle.None;
                        }
                    }
                }
                ShowValidationMessage(target, errorField);
            });
            container.Add(listenerField);
            
            for (int i = 0; i < maxToDisplay; i++)
            {
                if (target.parameters == null) break;
                if (i < target.parameters.Length)
                {
                    paramFields[i]
                        .BindProperty(property.FindPropertyRelative("parameters").GetArrayElementAtIndex(i));
                    paramFields[i].style.display = DisplayStyle.Flex;
                }
                else
                {
                    paramFields[i].Unbind();
                    paramFields[i].style.display = DisplayStyle.None;
                }
            }

            ShowValidationMessage(target, errorField);

            return container;
        }

        private void ShowValidationMessage(StringParameterBuilder target, Label errorField)
        {
            try
            {
                string s = target.ValidateValue();
                errorField.style.display = DisplayStyle.None;
            }
            catch (Exception e)
            {
                errorField.style.display = DisplayStyle.Flex;
            }
        }
    }
}