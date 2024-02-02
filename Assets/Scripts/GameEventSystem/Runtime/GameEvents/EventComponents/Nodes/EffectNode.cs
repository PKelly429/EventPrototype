using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    public class EffectNode : GameEventNode
    {
    }
    
    [NodeInfo("Log")]
    public class DebugLogNode : EffectNode
    {
        public string LogText;
        
        public override bool Execute()
        {
            Debug.Log(LogText);

            return true;
        }
    }
    
    [NodeInfo("Do Nothing")]
    public class DoNothingNode : EffectNode
    {
        public string LogText;
        
        public override bool Execute()
        {
            Debug.Log(LogText);

            return true;
        }
    }
}
