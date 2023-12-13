using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public abstract class VariableDefinition
{
    public string uniqueId;
    [SerializeField] private string _name;
    public abstract Type type { get; }
    public abstract object value { get; set; }
    
    public object GetField(string fieldName)
    {
        try
        {
            return value.GetType().GetProperty(fieldName).GetValue(value);
        }
        catch (Exception e)
        {
            return null;
        }
    }
    
    public event Action<string> onNameChanged;
    public string name 
    {
        get => _name;
        set
        {
            if (_name == value) return;
            _name = value;
            if ( onNameChanged != null ) {
                onNameChanged(value);
            }
        }
    }

    public bool IsValid()
    {
        return !string.IsNullOrEmpty(uniqueId);
    }

    public void GenerateId()
    {
        uniqueId = GUID.Generate().ToString();
    }
}

[AddTypeMenu("skip", 999)]
public class Variable<T> : VariableDefinition
{
    private T _value;
    public override Type type => typeof(T);

    public override object value
    {
        get => _value;
        set => _value = (T) value;
    }
}

[AddTypeMenu("Value/Int")]
public class IntVariable : Variable<int>
{
}

[AddTypeMenu("Value/Float")]
public class FloatVariable : Variable<float>
{
}

[AddTypeMenu("Value/Bool")]
public class BoolVariable : Variable<bool>
{
}

[AddTypeMenu("Value/String")]
public class StringVariable : Variable<string>
{
}

[AddTypeMenu("Value/Vector2")]
public class Vector2Variable : Variable<Vector2>
{
}

[AddTypeMenu("Value/Vector3")]
public class Vector3Variable : Variable<Vector3>
{
}

[AddTypeMenu("Value/Rotation")]
public class RotationVariable : Variable<Quaternion>
{
}