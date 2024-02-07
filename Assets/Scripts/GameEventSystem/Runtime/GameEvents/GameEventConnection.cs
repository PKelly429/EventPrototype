using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    [Serializable]
    public struct GameEventConnection
    {
        public string guid;
        public int portId;
        public string inputNodeId;
        public string outputNodeId;

        public GameEventConnection(int portId, string inputNodeId, string outputNodeId)
        {
            guid = System.Guid.NewGuid().ToString();
            this.portId = portId;
            this.inputNodeId = inputNodeId;
            this.outputNodeId = outputNodeId;
        }
    }
}
