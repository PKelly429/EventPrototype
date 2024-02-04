using System;
using System.Collections;
using System.Collections.Generic;
using GameEventSystem;
using UnityEngine;

[NodeInfo("Click On Object", "", 0, false, true)]
[NodeDescription("Stores clicked object in BBParam")]
public class ClickOnObjectTriggerNode : TriggerNode
{
    [DisplayField] public ClickableObjectReference objectReference;
    
    protected override void AddListener()
    {
        ClickableObject.OnObjectClicked += OnObjectClicked;
    }

    protected override void RemoveListener()
    {
        ClickableObject.OnObjectClicked -= OnObjectClicked;
    }
    
    private void OnObjectClicked(ClickableObject obj)
    {
        try
        {
            objectReference.SetValue(obj);
            Trigger();
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
}
