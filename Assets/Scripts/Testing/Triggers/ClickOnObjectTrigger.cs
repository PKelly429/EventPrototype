using System;
using System.Collections;
using System.Collections.Generic;
using GameEventSystem;
using UnityEngine;
#if UNITY_EDITOR
using GameEventSystem.GameEventSystem.GameEvents.Editor;
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
        try
        {
            objectReference.SetValue(obj);
            Activate();
        }
        catch (InvalidCastException e)
        {
            Debug.LogError($"Failed to cast {obj.Name}");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    
    
    protected override void SetObjReferences()
    {
        objectReference?.ResolveRef(localBlackboard);
    }

#if UNITY_EDITOR
    private bool _show;
    public override void DrawEditorWindowUI(IBlackboard localBlackboard)
    {
        string prefix = _show ? "[-]" : "[+]";
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button($"{prefix} OnClick", EditorUtils.UISubHeaderStyle))
        {
            _show = !_show;
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        if (_show)
        {
            EventEditorUtils.DrawLinkText("Clickable object {0} clicked.", localBlackboard, objectReference);
        }
    }
#endif
}
