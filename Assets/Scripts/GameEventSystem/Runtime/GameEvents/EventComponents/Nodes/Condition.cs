using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    [Serializable]
    
    public sealed class Condition : IBindable // wrapper class for drawing purposes: inherit from AbstractCondition
    {
        [SerializeField] private bool _redraw; // used to redraw the inspector when changed
        [SerializeReference] public AbstractCondition condition;
        
        public bool CheckCondition (GameEvent parentEvent)
        {
            if (condition == null) return true;
            
            return condition.CheckCondition(parentEvent);
        }

        public void Bind(IBlackboard blackboard)
        {
            if(condition!=null) condition.Bind(blackboard);
        }
    }

    public abstract class AbstractCondition : IBindable
    {
        public abstract bool CheckCondition(GameEvent parentEvent);
        public virtual void Bind(IBlackboard blackboard){ }
    }
}