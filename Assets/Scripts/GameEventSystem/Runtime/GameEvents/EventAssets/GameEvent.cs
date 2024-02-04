using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameEventSystem
{
    [CreateAssetMenu(fileName = "NewEvent", menuName = "GameEvents/Game Event", order = 1)]
    [Serializable]
    public class GameEvent : ScriptableObject
    {
        public List<GameEventNode> Nodes = new List<GameEventNode>();
        public List<GameEventConnection> AllConnections = new List<GameEventConnection>();

        public AssetBlackboard Blackboard;
        public AssetBlackboard LocalBlackboard;


        [ScriptableObjectIdAttribute] [SerializeField]
        private string _uniqueID;


        #region Editor Compatibility
#if UNITY_EDITOR
        public GameEventNode AddNode(Type type)
        {
            var node = ScriptableObject.CreateInstance(type) as GameEventNode;
            node.name = $"{type.Name}";
            node.GenerateGUID();
            Nodes.Add(node);

            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(node, this);
            }

            Undo.RegisterCreatedObjectUndo(node, "GameEvent (CreateNode)");
            AssetDatabase.SaveAssets();
            
            return node;
        }

        public void RemoveNode(GameEventNode node)
        {
            Nodes.Remove(node);
            AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(node);
            AssetDatabase.SaveAssets();
        }

        public AssetBlackboard CreateLocalBlackboard()
        {
            if (LocalBlackboard == null)
            {
                LocalBlackboard = ScriptableObject.CreateInstance(typeof(AssetBlackboard)) as AssetBlackboard;
                LocalBlackboard.name = "LocalBlackboard";
                if (!Application.isPlaying)
                {
                    AssetDatabase.AddObjectToAsset(LocalBlackboard, this);
                }
                AssetDatabase.SaveAssets();
            }

            Blackboard = LocalBlackboard;
            
            return Blackboard;
        }
#endif
    #endregion

        private List<GameEventNode> _activeNodes = new List<GameEventNode>();
        public void Setup()
        {
            foreach (var node in Nodes)
            {
                node.Setup();
            }
        }

        #region IBlackboard

        [SerializeField] private List<VariableDefinition> _definedVariables = new List<VariableDefinition>();
        
        public List<VariableDefinition> definedVariables => _definedVariables;
        public string uniqueID => _uniqueID;

        public VariableDefinition AddVariable(Type type)
        {
            return Blackboard.AddVariable(type);
            // VariableDefinition variable = ScriptableObject.CreateInstance(type) as VariableDefinition;
            // variable.Name = $"new{variable.type.Name}";
            //
            // variable.GenerateId();
            // definedVariables.Add(variable);
            //
            // AssetDatabase.AddObjectToAsset(variable, this);
            // AssetDatabase.SaveAssets();
            //
            // return variable;
        }

        public void RemoveVariable(VariableDefinition variable)
        {
            Blackboard.RemoveVariable(variable);
            
            // variable.Delete();
            // definedVariables.Remove(variable);
            //
            // AssetDatabase.RemoveObjectFromAsset(variable);
            // Undo.DestroyObjectImmediate(variable);
            // AssetDatabase.SaveAssets();
        }

        public void RemoveVariableById(string id)
        {
            var variable = GetVariableByID(id);
            if (variable == null) return;
            RemoveVariable(variable);
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

        // public void Setup()
        // {
        //     foreach (var trigger in triggers)
        //     {
        //         trigger.SetBlackboards(this);
        //         trigger.OnTriggerActivated += TryFireEvent;
        //         trigger.OnEnable();
        //     }
        //
        //     foreach (var effect in effects)
        //     {
        //         effect.SetBlackboards(this);
        //     }
        // }

        // public void OnEnable()
        // {
        //     foreach (var trigger in triggers)
        //     {
        //         trigger.SetBlackboards(this);
        //     }
        //
        //     foreach (var effects in effects)
        //     {
        //         effects.SetBlackboards(this);
        //     }
        // }

        // private void TryFireEvent()
        // {
        //     foreach (var condition in conditions)
        //     {
        //         if (!condition.Evaluate())
        //         {
        //             return;
        //         }
        //     }
        //
        //     FireEvent();
        // }
        //
        // public void FireEvent()
        // {
        //     foreach (var effect in effects)
        //     {
        //         effect.Execute();
        //     }
        // }
        //
        // public void AddTrigger(Trigger trigger)
        // {
        //     triggers.Add(trigger);
        //     trigger.SetBlackboards(this);
        // }
        //
        // public void AddCondition(Condition condition)
        // {
        //     conditions.Add(condition);
        //     condition.SetBlackboards(this);
        // }
        //
        // public void AddEffect(Effect effect)
        // {
        //     effects.Add(effect);
        //     effect.SetBlackboards(this);
        // }
    }
}