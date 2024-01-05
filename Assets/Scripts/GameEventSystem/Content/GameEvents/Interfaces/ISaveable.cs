using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    public interface ISaveable
    {
        object SaveData();
        void LoadData(object o);
    }
}