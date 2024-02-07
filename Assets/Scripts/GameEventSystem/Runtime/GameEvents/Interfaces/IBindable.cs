using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    public interface IBindable
    {
        public void Bind(IBlackboard blackboard);
    }
}
