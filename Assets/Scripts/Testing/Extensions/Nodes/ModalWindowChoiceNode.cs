using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    [NodeInfo("Choice Option")]
    [NodeDescription("Adds an option to a choice window")]
    [NodeConnectionInput(PortTypeDefinitions.PortTypes.Choice)]
    [NodeConnectionOutput(PortTypeDefinitions.PortTypes.Condition)]
    [NodeConnectionOutput(PortTypeDefinitions.PortTypes.Flow)]
    public class ModalWindowChoiceNode : EffectNode
    {
        public StringParameterBuilder text;

        protected override State OnUpdate()
        {
            return State.Success;
        }

        public void OnSelected()
        {
            Execute();
        }
    }
}
