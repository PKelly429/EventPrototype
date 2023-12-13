using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEvent", menuName = "GameEvents/Game Event", order = 1)]
[Serializable]
public class GameEvent : ScriptableObject, IBlackboard
{
    [SerializeReference] public List<AssetBlackboard> blackboards;
    [SerializeReference] public List<Trigger> triggers;
    [SerializeReference] public List<Condition> conditions;
    [SerializeReference] public List<Effect> effects;
    
    #region IBlackboard
    [SerializeReference] private List<VariableDefinition> _definedVariables = new List<VariableDefinition>();
    public List<VariableDefinition> definedVariables => _definedVariables;
    public int uniqueID => GetInstanceID();
    
    public void AddVariable(VariableDefinition variable)
    {
        variable.GenerateId();
        definedVariables.Add(variable);
    }

    public VariableDefinition GetVariableByID(string id)
    {
        foreach (var variable in definedVariables)
        {
            if (variable.uniqueId.Equals(id))
            {
                return variable;
            }
        }

        return null;
    }
    #endregion

    public void Setup()
    {
        foreach (var trigger in triggers)
        {
            trigger.SetBlackboards(this);
            trigger.OnTriggerActivated += TryFireEvent;
            trigger.OnEnable();
        }
        foreach (var effect in effects)
        {
            effect.SetBlackboards(this);
        }
    }
    
    public void OnEnable()
    {
        foreach (var trigger in triggers)
        {
            trigger.SetBlackboards(this);
        }
        foreach (var effects in effects)
        {
            effects.SetBlackboards(this);
        }
    }

    private void TryFireEvent()
    {
        foreach (var condition in conditions)
        {
            if (!condition.Evaluate())
            {
                return;
            }
        }

        FireEvent();
    }

    public void FireEvent()
    {
        foreach (var effect in effects)
        {
            effect.Execute();
        }
    }

    public void AddTrigger(Trigger trigger)
    {
        triggers.Add(trigger);
        trigger.SetBlackboards(this);
    }
    
    public void AddEffect(Effect effect)
    {
        effects.Add(effect);
        effect.SetBlackboards(this);
    }
}