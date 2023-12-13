using System;

[Serializable]
public abstract class Condition : EventComponent
{
    public abstract bool Evaluate();
}