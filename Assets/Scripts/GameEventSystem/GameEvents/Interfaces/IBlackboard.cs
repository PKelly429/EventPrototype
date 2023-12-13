using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public interface IBlackboard
{
    public List<VariableDefinition> definedVariables { get; }
    public int uniqueID { get; }
    
    public string name { get; }

    public void AddVariable(VariableDefinition variable);
    public VariableDefinition GetVariableByID(string id);
}
