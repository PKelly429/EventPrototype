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

    [ScriptableObjectIdAttribute] [SerializeField] private string _uniqueID;
    
    #region IBlackboard
    [SerializeReference] private List<VariableDefinition> _definedVariables = new List<VariableDefinition>();
    public List<VariableDefinition> definedVariables => _definedVariables;
    public string uniqueID => _uniqueID;
    [NonSerialized] public bool blackboardsSetup;
    
    public void AddVariable(VariableDefinition variable)
    {
        variable.GenerateId();
        definedVariables.Add(variable);
    }

    public void RemoveVariable(VariableDefinition variable)
    {
        variable.Delete();
        definedVariables.Remove(variable);
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
        SetupBlackboards();
        foreach (var trigger in triggers)
        {
            trigger.OnTriggerActivated += TryFireEvent;
            trigger.OnEnable();
        }
    }

    public void SetupBlackboards()
    {
        if (triggers == null || conditions == null || effects == null)
        {
            return;
        }
        
        foreach (var trigger in triggers)
        {
            trigger.SetBlackboards(this);
        }
        foreach (var condition in conditions)
        {
            condition.SetBlackboards(this);
        }
        foreach (var effect in effects)
        {
            effect.SetBlackboards(this);
        }

        blackboardsSetup = true;
    }
    
    public void OnEnable()
    {
        SetupBlackboards();
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
    
    public void AddCondition(Condition condition)
    {
        conditions.Add(condition);
        condition.SetBlackboards(this);
    }
    
    public void AddEffect(Effect effect)
    {
        effects.Add(effect);
        effect.SetBlackboards(this);
    }
}
