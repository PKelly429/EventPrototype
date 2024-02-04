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

    public class BlackboardFieldView : VisualElement
	{
		public AssetBlackboard blackboard;
		public VariableDefinition property;

		public Action<VisualElement, string> editTextRequested;
		public Action<BlackboardFieldView> onRemoveBlackboardProperty;

		public void CreateView(AssetBlackboard blackboard, VariableDefinition property)
		{
			this.blackboard = blackboard;
			this.property = property;

			VisualElement rowView = new VisualElement();
			rowView.style.flexDirection = FlexDirection.Row;
			
			Type valueType = this.property.type;

			var field = new GameEventBlackboardField
			{
				text = property.Name,
				typeText = valueType.Name,
				userData = property
			};
			rowView.Add(field);

			var deleteButton = new Button(() => onRemoveBlackboardProperty.Invoke(this))
			{
				text = "X"
			};
			deleteButton.AddToClassList("deleteButton");

			field.Add(deleteButton);

			Add(rowView);
			CreateField(field);
		}
		
		private void CreateField(VisualElement field)
		{
			string bindingPath = blackboard.GetBindingPath(property);
			
			PropertyField propertyField = new PropertyField();
			propertyField.bindingPath = bindingPath;
			propertyField.Bind(new SerializedObject(blackboard));
			propertyField.label = "Value:";
			//propertyField.ElementAt(0).style.minWidth = 50;

			var sa = new BlackboardRow(field, propertyField);
			Add(sa);
		}
	}
    
}
