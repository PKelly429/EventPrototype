using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ClickableObject : Entity
{
    public enum ObjectRole {RoleA, RoleB, RoleC}
    public static Action<ClickableObject> OnObjectClicked;

    public ObjectRole role;

    [ExposeProperty] public string Name => name;
    [ExposeProperty] public string Role => role.ToString();

    public void HandleClick()
    {
        OnObjectClicked?.Invoke(this);
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(ClickableObject))]
    public class ClickableObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ClickableObject obj = (ClickableObject)target;
            if (GUILayout.Button("CLICK"))
            {
                obj.HandleClick();
            }
        }
    }
#endif
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
