using System.Collections;
using System.Collections.Generic;
using GameEventSystem;
using UnityEngine;

namespace GameEventSystem
{
    [NodeInfo("ENTRY", "")]
    [NodeDescription("Event entry point")]
    [NodeConnectionInput(PortTypeDefinitions.PortTypes.Trigger)]
    [NodeConnectionOutput(PortTypeDefinitions.PortTypes.Flow)]
    [NodeConnectionOutput(PortTypeDefinitions.PortTypes.Condition)]
    public class RootNode : GameEventNode
    {
        public override bool IsRootNode => true;

        [DisplayField] public bool activeByDefault = true;
        [DisplayField] public bool canRepeat = true;
        
        protected override State OnUpdate()
        {
            return State.Success;
        }

        public void FireEvent()
        {
            if (runtimeGameEvent != null)
            {
                runtimeGameEvent.FireEvent();
            }
        }
        
        public override bool CheckConditions() // don't fail root node if conditions are not met
        {
            foreach (var conditionNode in conditionNodes)
            {
                if (!conditionNode.CheckConditions())
                {
                    return false;
                }
            }

            return true;
        }
        
        public override void PerformTestGraphFunction()
        {
#if DEBUG
            Debug.LogError("Must be in play mode and selecting a runtime event.");
#endif
            FireEvent();
        }
    }
}