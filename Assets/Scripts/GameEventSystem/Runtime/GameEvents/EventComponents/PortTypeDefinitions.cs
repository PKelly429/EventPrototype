using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    public static class PortTypeDefinitions
    {
        public enum PortTypes
        {
            Flow,
            Trigger,
            Condition,
            Choice
        }
        public class ChoicePort { };
        public class ConditionPort { };
        public class TriggerPort { };

        public static Type GetPortType(PortTypes type)
        {
            switch (type)
            {
                case PortTypes.Flow:
                    return typeof(bool);
                case PortTypes.Trigger:
                    return typeof(TriggerPort);
                case PortTypes.Condition:
                    return typeof(ConditionPort);
                case PortTypes.Choice:
                    return typeof(ChoicePort);
                default:
                    return typeof(bool);
                
            }
        }
        
        public static Color GetPortColour(PortTypes type)
        {
            switch (type)
            {
                case PortTypes.Flow:
                    return new Color(0.2666667f, 0.7411765f, 0.1960784f, 1);
                case PortTypes.Trigger:
                    return new Color(0.55f, 0.48f, 0.90f, 1);
                case PortTypes.Condition:
                    return new Color(0.7607843f, 0.2117647f, 0.08627451f, 1);
                case PortTypes.Choice:
                    return new Color(0.8823529f, 0.6941177f, 0.172549f, 1);
                default:
                    return Color.black;
                
            }
        }
    }
}
