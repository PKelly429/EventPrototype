using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    [System.Serializable]
    public class GameEventConnection
    {
        public string Id;
        public string InputNodeId;
        public string OutputNodeId;
        
        public GameEventConnection(string inputId, string outputId)
        {
            Id = System.Guid.NewGuid().ToString();
            InputNodeId = inputId;
            OutputNodeId = outputId;
        }
    }
}
