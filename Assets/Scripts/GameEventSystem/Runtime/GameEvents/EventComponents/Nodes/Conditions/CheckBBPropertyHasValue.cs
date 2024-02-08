using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    [Serializable]
    [AddTypeMenu("BBParam has value")]
    public class CheckBBPropertyHasValue : AbstractCondition
    {
        [DisplayField] public GeneralBBReference param;
        public override bool CheckCondition(GameEvent parentEvent)
        {
            return param.value != null;
        }

        public override void Bind(IBlackboard blackboard)
        {
            param.Bind(blackboard);
        }
    }
}