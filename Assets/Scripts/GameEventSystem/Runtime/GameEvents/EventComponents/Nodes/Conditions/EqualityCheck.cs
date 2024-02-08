using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameEventSystem
{
    [Serializable]
    [AddTypeMenu("Equality Check")]
    public class EqualityCheck : AbstractCondition
    {
        [DisplayField] public GeneralBBReference x;
        [DisplayField] public GeneralBBReference y;
        public override bool CheckCondition(GameEvent parentEvent)
        {
            if (x.value == null && y.value == null) return true;
            if (x.value == null) return false;
            if (y.value == null) return false;
            return x.value.Equals(y.value);
        }
        
        public override void Bind(IBlackboard blackboard)
        {
            x.Bind(blackboard);
            y.Bind(blackboard);
        }
    }
}