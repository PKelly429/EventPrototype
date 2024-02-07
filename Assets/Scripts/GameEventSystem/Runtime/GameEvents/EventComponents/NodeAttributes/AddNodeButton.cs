using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AddNodeButton : Attribute
    {
        public Type Type;

        public AddNodeButton(Type type)
        {
            this.Type = type;
        }
    }
}