using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    [NodeInfo("Wait Until", "FLOW")]
    [NodeDescription("Halt until condition met")]
    [NodeConnectionOutput(PortTypeDefinitions.PortTypes.Flow)]
    [NodeConnectionOutput(PortTypeDefinitions.PortTypes.Condition)]
    public class WaitUntilNode : EffectNode
    {
        protected override State OnUpdate()
        {
            if (CheckConditions())
            {
                return State.Success;
            }
            
            return State.Running;
        }
    }
}