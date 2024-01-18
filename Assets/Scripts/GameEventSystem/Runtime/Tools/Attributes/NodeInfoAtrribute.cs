using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    public class NodeInfoAtrribute : Attribute
    {
        public string NodeTitle { get; }
        public string NodeMenu { get; }

        public int Order { get; }

        public NodeInfoAtrribute(string nodeTitle, string nodeMenu = "", int order = 0)
        {
            NodeTitle = nodeTitle;
            NodeMenu = nodeMenu;
            Order = order;
        }

        static readonly char[] k_Separeters = new char[] { '/' };

        /// <summary>
        /// Returns the menu name split by the '/' separator.
        /// </summary>
        public string[] GetSplittedMenuName()
        {
            string fullPath = $"{NodeMenu}/{NodeTitle}";
            return !string.IsNullOrWhiteSpace(fullPath)
                ? NodeMenu.Split(k_Separeters, StringSplitOptions.RemoveEmptyEntries)
                : Array.Empty<string>();
        }
    }
}
