using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    public class NodeInfoAttribute : Attribute
    {
        public string NodeTitle { get; private set; }
        public string NodeMenu { get; private set; }
        public bool HasFlowInput { get; private set;}
        public bool HasFlowOutput { get; private set; }
        public int Order { get; private set; }

        public NodeInfoAttribute(string nodeTitle, string nodeMenu = "",  int order = 0, bool hasFlowInput = true, bool hasFlowOutput = true)
        {
            NodeTitle = nodeTitle;
            NodeMenu = nodeMenu;
            Order = order;
            HasFlowInput = hasFlowInput;
            HasFlowOutput = hasFlowOutput;
        }

        static readonly char[] k_Separeters = new char[] { '/' };

        /// <summary>
        /// Returns the menu name split by the '/' separator.
        /// </summary>
        public string[] GetSplittedMenuName()
        {
            if (string.IsNullOrEmpty(NodeMenu)) return new[] { NodeTitle };
            
            string fullPath = $"{NodeMenu}/{NodeTitle}";

            return string.IsNullOrWhiteSpace(fullPath)
                ? Array.Empty<string>()
                : fullPath.Split(k_Separeters, StringSplitOptions.RemoveEmptyEntries);
        }
    }
    
    public class NodeDescriptionAttribute : Attribute
    {
        public string NodeDescription { get; private set; }

        public NodeDescriptionAttribute(string nodeDescription)
        {
            NodeDescription = nodeDescription;

        }
    }
}
