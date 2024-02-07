using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


namespace GameEventSystem
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeConnectionInputAttribute : Attribute
    {
        public PortTypeDefinitions.PortTypes PortType { get; set; }
        public int Count { get; set; }

        public NodeConnectionInputAttribute(PortTypeDefinitions.PortTypes type, int count = 1)
        {
            PortType = type;
            Count = count;
        }
    }
    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class NodeConnectionOutputAttribute : Attribute
    {
        public PortTypeDefinitions.PortTypes PortType { get; set; }
        public int Count { get; set; }

        public NodeConnectionOutputAttribute(PortTypeDefinitions.PortTypes type, int count = 1)
        {
            PortType = type;
            Count = count;
        }
    }
}

