using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class EventComponent
{
    protected IBlackboard localBlackboard;

    public virtual void SetBlackboards(IBlackboard blackboard)
    {
        localBlackboard = blackboard;
        SetObjReferences();
    }

    protected virtual void SetObjReferences()
    {
        
    }
#if UNITY_EDITOR
    public abstract void DrawEditorWindowUI();
#endif
}
