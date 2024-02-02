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

        public NodeInfoAttribute(string nodeTitle, string nodeMenu = "", int order = 0, bool hasFlowInput = true, bool hasFlowOutput = true)
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
    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CustomNodeStyleAttribute : Attribute
    {
        public string style;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_name"></param>
        public CustomNodeStyleAttribute(string style)
        {
            this.style = style;
        }
    }
}
