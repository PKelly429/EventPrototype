using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameEventSystem
{
    [Serializable]
    public abstract class VariableDefinition : ScriptableObject
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

        public event Action onRemoved;
        public event Action<string> onNameChanged;

        public string name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                if (onNameChanged != null)
                {
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

        public void Delete()
        {
            onRemoved?.Invoke();
        }
    }

    [AddTypeMenu("skip", 999)]
    public abstract class Variable<T> : VariableDefinition
    {
        private T _value;
        public override Type type => typeof(T);

        public override object value
        {
            get => _value;
            set => _value = (T)value;
        }
    }

    [AddTypeMenu("BuiltIn/Int")]
    public class IntVariable : Variable<int>
    {
    }

    [AddTypeMenu("BuiltIn/Float")]
    public class FloatVariable : Variable<float>
    {
    }

    [AddTypeMenu("BuiltIn/Bool")]
    public class BoolVariable : Variable<bool>
    {
    }

    [AddTypeMenu("BuiltIn/String")]
    public class StringVariable : Variable<string>
    {
    }

    [AddTypeMenu("BuiltIn/Colour")]
    public class ColourVariable : Variable<Color>
    {
    }

    [AddTypeMenu("BuiltIn/Vector2")]
    public class Vector2Variable : Variable<Vector2>
    {
    }

    [AddTypeMenu("BuiltIn/Vector3")]
    public class Vector3Variable : Variable<Vector3>
    {
    }

    [AddTypeMenu("BuiltIn/Rotation")]
    public class RotationVariable : Variable<Quaternion>
    {
    }
    
    [AddTypeMenu("BuiltIn/Object")]
    public class ObjectVariable : Variable<UnityEngine.Object>
    {
    }
}