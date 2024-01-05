using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    [Serializable]
    public abstract class Effect : EventComponent
    {
        public abstract bool Execute();
    }
}