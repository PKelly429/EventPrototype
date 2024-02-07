using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameEventSystem
{
    [CreateAssetMenu(fileName = "NewEvent", menuName = "GameEvents/Game Event", order = 1)]
    [Serializable]
    public class GameEvent : ScriptableObject
    {
        [Serializable]
        public enum EventState
        {
            Disabled,
            Ready,
            Running,
            Complete
        }
        
        public List<GameEventNode> nodes = new List<GameEventNode>();
        public RootNode rootNode;
        [NonSerialized] private EventState _state;
        public float startTime { get; private set; }

        public AssetBlackboard blackboard;
        [SerializeField] private AssetBlackboard _localBlackboard;


        [ScriptableObjectIdAttribute] [SerializeField] private string _uniqueID;
        public string UniqueId => _uniqueID;

        public bool CanEventRepeat => rootNode.canRepeat;

        [NonSerialized] private Dictionary<string, GameEventNode> nodeLookup = new Dictionary<string, GameEventNode>();

        private bool _isInitialising;
        private bool _delayedTrigger;

        private bool _triggersSubscribed;
        private List<TriggerNode> _triggerNodes = new List<TriggerNode>();
        private List<GameEventNode> _activeNodes = new List<GameEventNode>();

        public void Setup() // TODO: pass in saved state on setup
        {
            _isInitialising = true;
            _delayedTrigger = false;

            foreach (var node in nodes)
            {
                node.Setup(this);
            }
            
            //TODO: Save / load event state
            if(rootNode.activeByDefault) SetState(EventState.Ready);

            _isInitialising = false;
            if (_delayedTrigger)
            {
                _delayedTrigger = false;
                FireEvent();
            }
        }

        public void SetState(EventState state)
        {
            if (_state == state) return;
            _state = state;

            if (_state == EventState.Running)
            {
                startTime = Time.time;
                EventManager.SetEventTriggered(this);
            }
            bool subscribeToEvents = _state == EventState.Ready;
            if (subscribeToEvents == _triggersSubscribed) return;

            if (subscribeToEvents)
            {
                foreach (var triggerNode in _triggerNodes)
                {
                    triggerNode.AddTriggerListener();
                }
                
            }
            else
            {
                foreach (var triggerNode in _triggerNodes)
                {
                    triggerNode.RemoveTriggerListener();
                }
            }

            _triggersSubscribed = subscribeToEvents;
        }

        public bool CheckConditions()
        {
            return rootNode.CheckConditions();
        }

        public void FireEvent()
        {
            if (_isInitialising)
            {
                _delayedTrigger = true;
                return;
            }

            SetState(EventState.Running);

            rootNode.Execute();   
        }
        
        #region Runtime Init
        public GameEvent Clone()
        {
            GameEvent clone = Instantiate(this);
            if (blackboard != null)
            {
                clone.blackboard = blackboard == _localBlackboard ? Instantiate(_localBlackboard) : AssetBlackboard.GetBlackboard(blackboard.uniqueID);
            }

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
            if (node.IsRootNode) rootNode = node as RootNode;
            if(node.IsTriggerNode) _triggerNodes.Add(node as TriggerNode);
            nodeLookup.Add(node.Id, node);
        }

        public void Bind()
        {
            foreach (var node in nodes)
            {
                node.BindBlackboard(blackboard);
            }
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        public GameEventNode GetNode(string id)
        {
            return nodeLookup[id];
        }
        #endregion

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

    }
}