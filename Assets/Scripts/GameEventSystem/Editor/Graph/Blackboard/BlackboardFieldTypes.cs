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
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class BlackboardPropertyTypeAttribute : Attribute
    {
        public Type type;

        /// <summary>
		/// 
		/// </summary>
		/// <param name="_name"></param>
		public BlackboardPropertyTypeAttribute(Type _type)
        {
            type = _type;
        }
    }

    public abstract class BlackboardFieldView : VisualElement
	{
		public AssetBlackboard blackboard;
		public VariableDefinition property;

		public Action<VisualElement, string> editTextRequested;
		public Action<BlackboardFieldView> onRemoveBlackboardProperty;

		public abstract void CreateField(VisualElement field);

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
		
		public void CreatePropertyField<T, ElTy>(VisualElement field, Variable<T> property)
		{
			string bindingPath = blackboard.GetBindingPath(property);

			BaseField<T> propertyField = Activator.CreateInstance(typeof(ElTy)) as BaseField<T>;
			propertyField.label = "Value:";
			if (!string.IsNullOrEmpty(bindingPath))
			{
				propertyField.bindingPath = bindingPath;
				propertyField.Bind(new SerializedObject(blackboard));
			}
			propertyField.ElementAt(0).style.minWidth = 50;
			var sa = new BlackboardRow(field, propertyField);
			Add(sa);
		}

		// public void CreateObjectPropertyField<T>(BlackboardField field, Variable<T> property) where T : UnityEngine.Object
		// {
		// 	ObjectField propertyField = new ObjectField("Value:");
		// 	propertyField.objectType = typeof(T);
		// 	propertyField.bindingPath = "abstractData";
		// 	propertyField.Bind(new SerializedObject(property));
		// 	propertyField.ElementAt(0).style.minWidth = 50;
		// 	var sa = new BlackboardRow(field, propertyField);
		// 	Add(sa);
		// }
	}



    
    [BlackboardPropertyType(typeof(BoolVariable))]
    public class BlackboardBoolPropertyView : BlackboardFieldView
    {
	    public override void CreateField(VisualElement field)
	    {
		    BoolVariable localProperty = (BoolVariable)property;
		    CreatePropertyField<bool, Toggle>(field, localProperty);
	    }
    }
    
    [BlackboardPropertyType(typeof(IntVariable))]
    public class BlackboardIntPropertyView : BlackboardFieldView
    {
	    public override void CreateField(VisualElement field)
	    {
		    IntVariable localProperty = (IntVariable)property;
		    CreatePropertyField<int, IntegerField>(field, localProperty);
	    }
    }
    
    [BlackboardPropertyType(typeof(FloatVariable))]
    public class BlackboardFloatPropertyView : BlackboardFieldView
    {
	    public override void CreateField(VisualElement field)
	    {
		    FloatVariable localProperty = (FloatVariable)property;
		    CreatePropertyField<float, FloatField>(field, localProperty);
	    }
    }
    
    [BlackboardPropertyType(typeof(StringVariable))]
    public class BlackboardStringPropertyView : BlackboardFieldView
    {
	    public override void CreateField(VisualElement field)
	    {
		    StringVariable localProperty = (StringVariable)property;
		    CreatePropertyField<string, TextField>(field, localProperty);
	    }
    }
    
    [BlackboardPropertyType(typeof(ColourVariable))]
    public class BlackboardColorPropertyView : BlackboardFieldView
    {
	    public override void CreateField(VisualElement field)
	    {
		    ColourVariable localProperty = (ColourVariable)property;
		    CreatePropertyField<Color, ColorField>(field, localProperty);
	    }
    }
    
    [BlackboardPropertyType(typeof(Vector2Variable))]
    public class BlackboardVector2PropertyView : BlackboardFieldView
    {
	    public override void CreateField(VisualElement field)
	    {
		    Vector2Variable localProperty = (Vector2Variable)property;
		    CreatePropertyField<Vector2, Vector2Field>(field, localProperty);
	    }
    }
    
    [BlackboardPropertyType(typeof(Vector3Variable))]
    public class BlackboardVector3PropertyView : BlackboardFieldView
    {
	    public override void CreateField(VisualElement field)
	    {
		    Vector3Variable localProperty = (Vector3Variable)property;
		    CreatePropertyField<Vector3, Vector3Field>(field, localProperty);
	    }
    }
    
    [BlackboardPropertyType(typeof(ObjectVariable))]
    public class BlackboardObjectView : BlackboardFieldView
    {
	    public override void CreateField(VisualElement field)
	    {
		    ObjectVariable localProperty = (ObjectVariable)property;
		    CreatePropertyField<UnityEngine.Object, ObjectField>(field, localProperty);
	    }
    }
}
