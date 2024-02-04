using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    // [System.Serializable]
    // public class GameEventConnection
    // {
    //     public string Id;
    //     public string InputNodeId;
    //     public string OutputNodeId;
    //     
    //     public GameEventConnection(string inputId, string outputId)
    //     {
    //         Id = System.Guid.NewGuid().ToString();
    //         InputNodeId = inputId;
    //         OutputNodeId = outputId;
    //     }
    // }

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
