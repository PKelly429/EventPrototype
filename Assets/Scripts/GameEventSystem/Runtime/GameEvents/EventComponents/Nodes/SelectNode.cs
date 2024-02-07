using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    [NodeInfo("IF THEN ELSE", "")]
    [NodeDescription("Selects output depending on condition")]
    [NodeConnectionOutput(PortTypeDefinitions.PortTypes.Flow, 2)]
    [NodeConnectionOutput(PortTypeDefinitions.PortTypes.Condition)]
    public class SelectNode : EffectNode
    {
        protected override State OnUpdate()
        {
            return State.Success;
        }
        
        protected override void OnStop()
        {
            // the left children are connections on port 0, the right children are on port 1
            // will go wrong if the ports are created in a different order
            
            int selectedPortId = CheckConditions() ? 0 : 1;

            state = State.Success;
            foreach (var connection in connections)
            {
                if (connection.portId == selectedPortId)
                {
                    FindChildWithId(connection.outputNodeId)?.Execute();
                }
            }
        }

        private GameEventNode FindChildWithId(string id)
        {
            return children.Find(node => node.Id.Equals(id));
        }
    }
}