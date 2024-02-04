using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    public class BlackboardFieldAttribute : Attribute
    {
        public Type type;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_name"></param>
        public BlackboardFieldAttribute(Type _type)
        {
            type = _type;
        }
    }
}
