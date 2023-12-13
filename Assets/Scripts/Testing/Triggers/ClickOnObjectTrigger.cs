using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using GameEventSystem.GameEvents.Editor;
using UnityEditor;
#endif

[Serializable]
public class ClickOnObjectTrigger : Trigger
{
    public ClickableObjectReference objectReference;
    
    public override void OnEnable()
    {
        ClickableObject.OnObjectClicked += OnObjectClicked;
    }

    public override void OnDisable()
    {
        ClickableObject.OnObjectClicked -= OnObjectClicked;
    }
    
    private void OnObjectClicked(ClickableObject obj)
    {
        objectReference.SetValue(obj);
        Activate();
    }
    
    
    protected override void SetObjReferences()
    {
        objectReference?.ResolveRef(localBlackboard);
    }

#if UNITY_EDITOR
    public override void DrawEditorWindowUI()
    {
        EventEditorUtils.DrawLinkText("Clickable object {0} clicked.", localBlackboard, objectReference);
    }
    #endif
}
