using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GameEventSystem
{
    public static class ShowEntitySearchWindow
    {
        public static void Open(IBlackboard blackboard, VariableReference toSet)
        {
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
                new VariableSearchProvider(blackboard, (selection) => { toSet.SetRef(selection); },
                    toSet.type));
        }
    }
}