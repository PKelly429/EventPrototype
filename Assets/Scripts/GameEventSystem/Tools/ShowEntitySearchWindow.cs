using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public static class ShowEntitySearchWindow
{
    public static void Open(IBlackboard blackboard, List<IBlackboard> extraBlackboards, VariableReference toSet)
    {
        SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
            new VariableSearchProvider(blackboard, extraBlackboards, (selection) => { toSet.SetRef(selection);}, toSet.type));
    }
    
    
}
