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
        public List<GameEventNode> nodes = new List<GameEventNode>();
        //public List<GameEventConnection> AllConnections = new List<GameEventConnection>();

        public AssetBlackboard blackboard;
        [SerializeField] private AssetBlackboard _localBlackboard;


        [ScriptableObjectIdAttribute] [SerializeField]
        private string _uniqueID;

        [NonSerialized] private Dictionary<string, GameEventNode> nodeLookup = new Dictionary<string, GameEventNode>();
        
        private List<GameEventNode> _activeNodes = new List<GameEventNode>();

        public GameEvent Clone()
        {
            GameEvent clone = Instantiate(this);
            clone.blackboard = AssetBlackboard.GetBlackboard(blackboard.uniqueID);
            clone.nodes.Clear();
            foreach (var node in nodes)
            {
                clone.AddRuntimeNode(node.Clone());
            }

            clone.Bind();
            return clone;
        }

        private void AddRuntimeNode(GameEventNode node)
        {
            nodes.Add(node);
            nodeLookup.Add(node.Id, node);
        }
        
        public void Setup()
        {
            foreach (var node in nodes)
            {
                node.Setup(this);
            }
        }
        
        public void Bind() 
        {
            foreach (var node in nodes)
            {
                node.BindBlackboard(blackboard);
            }
        }

        public GameEventNode GetNode(string id)
        {
            return nodeLookup[id];
        }

        #region Editor Compatibility
#if UNITY_EDITOR
        public GameEventNode AddNode(Type type)
        {
            var node = ScriptableObject.CreateInstance(type) as GameEventNode;
            node.name = $"{type.Name}";
            node.GenerateGUID();
            nodes.Add(node);

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
            nodes.Remove(node);
            AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(node);
            AssetDatabase.SaveAssets();
        }

        public AssetBlackboard CreateLocalBlackboard()
        {
            if (_localBlackboard == null)
            {
                _localBlackboard = ScriptableObject.CreateInstance(typeof(AssetBlackboard)) as AssetBlackboard;
                _localBlackboard.name = "LocalBlackboard";
                if (!Application.isPlaying)
                {
                    AssetDatabase.AddObjectToAsset(_localBlackboard, this);
                }
                AssetDatabase.SaveAssets();
            }

            blackboard = _localBlackboard;
            
            return blackboard;
        }
#endif
    #endregion

    #region IBlackboard

        [SerializeField] private List<VariableDefinition> _definedVariables = new List<VariableDefinition>();
        
        public List<VariableDefinition> definedVariables => _definedVariables;
        public string uniqueID => _uniqueID;

        public VariableDefinition AddVariable(Type type)
        {
            return blackboard.AddVariable(type);
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
            blackboard.RemoveVariable(variable);
            
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