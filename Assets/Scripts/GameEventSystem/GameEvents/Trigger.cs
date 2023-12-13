using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Trigger : EventComponent
{
    public delegate void TriggerActivated();
    public event TriggerActivated OnTriggerActivated;

    public abstract void OnEnable();

    public abstract void OnDisable();

    protected virtual void Activate()
    {
        OnTriggerActivated?.Invoke();
    }
}
