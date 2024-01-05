using System;

namespace GameEventSystem
{
    [Serializable]
    public abstract class Condition : EventComponent
    {
        public abstract bool Evaluate();
    }
}