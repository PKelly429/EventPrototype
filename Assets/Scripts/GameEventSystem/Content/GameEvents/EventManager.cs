using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    public class EventManager : MonoBehaviour
    {
        public List<GameEvent> gameEvents;
        void Start()
        {
            foreach (var gameEvent in gameEvents)
            {
                gameEvent.Setup();
            }
        }
    }
}

