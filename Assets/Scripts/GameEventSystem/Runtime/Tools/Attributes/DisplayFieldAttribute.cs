using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DisplayFieldAttribute : Attribute
    {
        public DisplayFieldAttribute(){}
    }
}
