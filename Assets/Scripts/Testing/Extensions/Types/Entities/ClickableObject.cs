using System;
using System.Collections;
using System.Collections.Generic;
using GameEventSystem;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ClickableObject : Entity
{
    public static Action<ClickableObject> OnObjectClicked;
    
    [ExposePropertyToBlackBoard]
    public string Name
    {
        get
        {
            return name;
        }
        set
        {
            name = value;
        }
    }

    public void HandleClick()
    {
        OnObjectClicked?.Invoke(this);
    }
    
}

[AddTypeMenu("Entity/Clickable Object", 1)]
[Serializable]
public class ClickableObjectDefinition : Variable<ClickableObject>
{
}

[Serializable]
public class ClickableObjectReference : VariableRef<ClickableObject>
{
}