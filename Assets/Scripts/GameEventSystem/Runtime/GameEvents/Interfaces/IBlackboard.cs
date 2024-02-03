using System;
using System.Collections.Generic;

namespace GameEventSystem
{
    public interface IBlackboard
    {
        public List<VariableDefinition> definedVariables { get; }
        public string uniqueID { get; }

        public string name { get; }

        public VariableDefinition AddVariable(Type type);
        public void RemoveVariable(VariableDefinition variable);
        public void RemoveVariableById(string id);
        public VariableDefinition GetVariableByID(string id);
    }
}