using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GameEventSystem.Editor
{
    public class GameEventEditorNode : Node
    {
        public GameEventEditorNode()
        {
            this.AddToClassList("game-event-node");
        }
    }
}
