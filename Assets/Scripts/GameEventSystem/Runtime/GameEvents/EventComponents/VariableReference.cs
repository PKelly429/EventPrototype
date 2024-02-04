using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace GameEventSystem
{
    [System.Serializable]
    public abstract class VariableReference
    {
        public string refId;
        public string blackboardId;
        public string name;
        public string propertyName;
        
        public abstract object localValue { get; set; }

        private VariableDefinition _variableDefinition;

        public VariableDefinition variableDefinition
        {
            get => _variableDefinition;
            set
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    if (_variableDefinition != null)
                    {
                        _variableDefinition.onNameChanged -= OnDefinitionNameChanged;
                        value.onRemoved -= OnDefinitionRemoved;
                    }

                    if (value != null)
                    {
                        value.onNameChanged += OnDefinitionNameChanged;
                        value.onRemoved += OnDefinitionRemoved;
                    }

                    UpdateName();
                }
#endif
                _variableDefinition = value;
            }
        }

        private void UpdateName()
        {
            if (_variableDefinition != null)
            {
                name = string.IsNullOrEmpty(propertyName)
                    ? _variableDefinition.Name
                    : $"{_variableDefinition.Name}.{propertyName}";
            }
            else
            {
                name = string.Empty;
            }
        }

        public abstract Type type { get; }

        public object value
        {
            get
            {
                if (_variableDefinition == null)
                {
                    ResolveRef(null);
                    if (_variableDefinition == null)
                    {
                        return localValue;
                    }
                }

                if (!string.IsNullOrEmpty(propertyName))
                {
                    return _variableDefinition.GetField(propertyName);
                }

                return _variableDefinition.value;
            }
        }

        public bool HasRef => !string.IsNullOrEmpty(refId);

        public void RemoveRef()
        {
            refId = string.Empty;
            name = string.Empty;
            propertyName = string.Empty;
        }

#if UNITY_EDITOR
        public void SetRef(VariableSearchProvider.VariableSelection selection)
        {
            variableDefinition = selection.variable;
            blackboardId = selection.blackboard.uniqueID;
            propertyName = selection.propertyName;
            refId = variableDefinition == null ? string.Empty : _variableDefinition.uniqueId;

            UpdateName();
        }

        private void OnDefinitionNameChanged(string obj)
        {
            UpdateName();
        }

        private void OnDefinitionRemoved()
        {
            RemoveRef();
        }
#endif

        public void SetValue(object obj)
        {
            if (_variableDefinition == null)
            {
                ResolveRef(null);
                if (_variableDefinition == null)
                {
                    return;
                }
            }

            _variableDefinition.value = obj;
        }

        public string GetName()
        {
            if (string.IsNullOrEmpty(refId))
            {
                return $"[not set]";
            }

            return name;
        }

        public void ResolveRef(IBlackboard blackboard)
        {
            if (string.IsNullOrEmpty(refId)) return;
            
            if (blackboard == null) return;
            var varDef = blackboard.GetVariableByID(refId);
            if (varDef != null)
            {
                if (varDef.type == type)
                {
                    variableDefinition = varDef;   
                    UpdateName();
                    return;
                }
            }
            
            RemoveRef();
        }
    }

    public abstract class VariableRef<T> : VariableReference
    {
        public override Type type => typeof(T);
        public T GetValue() => (T)value;

        [SerializeField] private T _localValue;

        public override object localValue
        {
            get => _localValue;
            set => _localValue = (T)value;
        }
    }
    
    [System.Serializable]
    public class IntParameter : VariableRef<int>
    {
    }
    
    [System.Serializable]
    public class BoolParameter : VariableRef<bool>
    {
    }
    
    [System.Serializable]
    public class StringParameter : VariableRef<string>
    {
    }
}