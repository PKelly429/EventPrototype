using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameEventSystem
{
    #if UNITY_EDITOR
    public static class BlackboardFieldUtil
    {
	    private static Dictionary<Type, BlackboardFieldConstructor> blackboardFieldTypes = new Dictionary<Type, BlackboardFieldConstructor>();

	    public static VisualElement GetFieldForParameter(AssetBlackboard blackboard, VariableDefinition param)
	    {
		    if(blackboardFieldTypes == null) InitBlackboardFields();

		    Type paramType = param.GetType();

		    if (!blackboardFieldTypes.ContainsKey(paramType))
		    {
			    InitBlackboardFields();
		    }

		    if (blackboardFieldTypes.ContainsKey(paramType))
		    {
			    return blackboardFieldTypes[paramType].GetPropertyField(blackboard, param);
		    }

		    return new Label("Could not create param.");
	    }

	    public static VisualElement GetFieldForParameter(GameEventNode parentNode, VariableReference param, string bindingPath)
	    {
		    if(blackboardFieldTypes == null) InitBlackboardFields();

		    Type paramType = param.GetType();

		    if (!blackboardFieldTypes.ContainsKey(paramType))
		    {
			    InitBlackboardFields();
		    }

		    if (blackboardFieldTypes.ContainsKey(paramType))
		    {
			    return blackboardFieldTypes[paramType].GetPropertyField(parentNode, param, bindingPath);
		    }

		    return new Label("Could not create param.");
	    }

	    public static BaseField<T> GetPropertyField<T, TU>()
	    {
		    return Activator.CreateInstance(typeof(TU)) as BaseField<T>;
	    }
	    
	    private static void InitBlackboardFields()
	    {
		    if (blackboardFieldTypes == null)
		    {
			    blackboardFieldTypes = new Dictionary<Type, BlackboardFieldConstructor>();
		    }
		    blackboardFieldTypes.Clear();
            
		    var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
		    foreach (var assembly in assemblies)
		    {
			    var types = assembly.GetTypes();
			    foreach (var type in types)
			    {
				    if (type.IsSubclassOf(typeof(BlackboardFieldConstructor)) == true && type.IsAbstract == false)
				    {
					    BlackboardFieldAttribute fieldInfo = type.GetCustomAttribute<BlackboardFieldAttribute>();
					    if (fieldInfo == null) continue;
					    blackboardFieldTypes.Add(fieldInfo.type, Activator.CreateInstance(type) as BlackboardFieldConstructor);
				    }
			    }
		    }
	    }
    }
#endif

	[BlackboardFieldAttribute(typeof(BoolParameter))]
	public class BoolParamFieldConstructor : GenericBlackboardFieldConstructor<bool, Toggle>
	{
		
	}
	[BlackboardFieldAttribute(typeof(BoolVariable))]
	public class BoolVariableFieldConstructor : GenericBlackboardFieldConstructor<bool, Toggle>
	{
		
	}
	
	[BlackboardFieldAttribute(typeof(StringParameter))]
	public class StringParamFieldConstructor : GenericBlackboardFieldConstructor<string, TextField>
	{
		
	}
	[BlackboardFieldAttribute(typeof(StringVariable))]
	public class StringVariableFieldConstructor : GenericBlackboardFieldConstructor<string, TextField>
	{
		
	}

	[BlackboardFieldAttribute(typeof(IntParameter))]
	public class IntParamFieldConstructor : GenericBlackboardFieldConstructor<int, IntegerField>
	{
		
	}
	[BlackboardFieldAttribute(typeof(IntVariable))]
	public class IntVariableFieldConstructor : GenericBlackboardFieldConstructor<int, IntegerField>
	{
		
	}



	// Base Classes
	
	public abstract class BlackboardFieldConstructor
	{
		public abstract VisualElement GetPropertyField(AssetBlackboard blackboard, VariableDefinition param);
		public abstract VisualElement GetPropertyField(GameEventNode parentNode, VariableReference param, string bindingPath);
	}

	public abstract class GenericBlackboardFieldConstructor<T, TU> : BlackboardFieldConstructor
	{
		public override VisualElement GetPropertyField(AssetBlackboard blackboard, VariableDefinition param)
		{
			return _getPropertyField(blackboard, param);
		}
		public override VisualElement GetPropertyField(GameEventNode parentNode, VariableReference param, string bindingPath)
		{
			return _getPropertyField(parentNode, param, bindingPath);
		}
		
		private BaseField<T> _getPropertyField(AssetBlackboard blackboard, VariableDefinition param)
		{
			var field= Activator.CreateInstance(typeof(TU)) as BaseField<T>;
			field.label = "Value:";
			field.bindingPath = blackboard.GetBindingPath(param);
			field.Bind(new SerializedObject(blackboard));

			field.ElementAt(0).style.minWidth = 50;
			return field;
		}
		
		private BaseField<T> _getPropertyField(GameEventNode parentNode, VariableReference param, string bindingPath)
		{
			var field = Activator.CreateInstance(typeof(TU)) as BaseField<T>;
			
			field.label = GetDisplayString(bindingPath);
			field.bindingPath = bindingPath;
			field.Bind(new SerializedObject(parentNode));

			field.ElementAt(0).style.minWidth = 50;
			return field;
		}

		private string GetDisplayString(string fieldName)
		{
			if (string.IsNullOrEmpty(fieldName)) return string.Empty;
			
			string str = fieldName.Replace("_", string.Empty);
			return $"{str[0].ToString().ToUpper()}{str.Substring(1)}";
		}
	}
}
