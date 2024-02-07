using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    [Serializable]
    [AddTypeMenu("Check Bool")]
    public class CheckBoolCondition : AbstractCondition
    {
        public BoolParameter testParameter;
        public override bool CheckCondition(GameEvent parentEvent)
        {
            return testParameter.GetValue();
        }
    }
}