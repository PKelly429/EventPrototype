using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{ 
    public abstract class ConditionNode : GameEventNode
    {
        protected abstract bool CheckCondition();
        protected override State OnUpdate()
        {
            return state;
        }
    }
}
