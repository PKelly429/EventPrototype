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
    }
}