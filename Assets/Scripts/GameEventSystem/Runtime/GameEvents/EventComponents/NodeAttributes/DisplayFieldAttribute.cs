using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    /// <summary>
    /// Display a field inside the node, instead of the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DisplayFieldAttribute : Attribute
    {
        public DisplayFieldAttribute(){}
    }
}
