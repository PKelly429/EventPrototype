using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    [Serializable]
    public class Condition
    {
        [SerializeField] private bool _redraw; // used to redraw the inspector when changed
        [SerializeReference] public AbstractCondition condition;
        
        public bool CheckCondition ()
        {
            if (condition == null) return true;
            
            return condition.CheckCondition();
        }
    }
    
    public abstract class AbstractCondition
    {
        public abstract bool CheckCondition();
    }
}