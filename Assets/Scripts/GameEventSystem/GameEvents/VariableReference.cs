using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public abstract class VariableReference
{
    public string refId;
    public string blackboardId;
    public string name;
    public string propertyName;

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
            name = string.IsNullOrEmpty(propertyName) ? _variableDefinition.name : $"{_variableDefinition.name}.{propertyName}";
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
                    return null;
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
        refId = string.Empty;
        name = string.Empty;
        propertyName = string.Empty;
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

    public void ResolveRef(IBlackboard localBlackboard)
    {
        if (string.IsNullOrEmpty(refId)) return;
        IBlackboard blackboard = localBlackboard;
        if (localBlackboard == null || localBlackboard.uniqueID != blackboardId)
        {
            blackboard = AssetBlackboard.GetBlackboard(blackboardId);
        }
        if (blackboard == null) return;
        var varDef = blackboard.GetVariableByID(refId);
        if (varDef != null)
        {
            variableDefinition = varDef;
        }
        UpdateName();
    }
}

public class VariableRef<T> : VariableReference
{
    public override Type type => typeof(T);
    public T GetValue() => (T)value;
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