using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    [NodeInfo("OR")]
    [NodeConnectionInput(PortTypeDefinitions.PortTypes.Condition, 1)]
    [NodeConnectionOutput(PortTypeDefinitions.PortTypes.Condition, 1)]
    public class ORNode : GameEventNode
    {
        public override bool CheckConditions()
        {
            foreach (var condition in conditionNodes)
            {
                if (condition.CheckConditions())
                {
                    return true;
                }
            }
            
            return false;
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
            if (_runtimeGameEvent == null)
            {
                Debug.LogError("Trying to check condition on event that is not running.");
                return;
            }
#endif
            bool success = CheckConditions();
            if (success)
            {
                state = State.Success;
            }
            state = State.Failure;
            Debug.Log($"CheckCondition: {success}");
        }
    }
}