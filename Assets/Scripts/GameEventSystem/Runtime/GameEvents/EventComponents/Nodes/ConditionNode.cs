using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    [NodeInfo("Condition")]
    [NodeConnectionInput(PortTypeDefinitions.PortTypes.Condition, 1)]
    [NodeConnectionOutput(PortTypeDefinitions.PortTypes.Condition, 0)]
    public class ConditionNode : GameEventNode
    {
        public override bool IsConditionNode => true;

        [DisplayField] public bool invert;
        [DisplayField] public Condition condition;
        
        public override bool CheckConditions()
        {
            bool result = condition == null || (condition.CheckCondition(runtimeGameEvent) != invert);
            
            SetState(result ? State.Success : State.Failure);
            
            return result;
        }
        
        protected override State OnUpdate()
        {
            if (CheckConditions())
            {
                return State.Success;
            }
            return State.Failure;
        }
        
        public override void PerformTestGraphFunction()
        {
#if DEBUG
            if (runtimeGameEvent == null)
            {
                Debug.LogError("Trying to check condition on event that is not running.");
                return;
            }
#endif
            Debug.Log($"CheckCondition: {CheckConditions()}");
        }
    }
}
