using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    public class GameEventRunner : MonoBehaviour
    {
        [SerializeField] private GameEvent _gameEvent;

        [NonSerialized] private GameEvent _runningEvent;

        public void OnEnable()
        {
            ExecuteEvent();
        }

        private void ExecuteEvent()
        {
            if (_gameEvent == null) return;
            
            _runningEvent = Instantiate(_gameEvent);
            _runningEvent.Setup();
        }
        
        public static void StartEvent(GameEvent gameEvent, Transform parent = null)
        {
            GameObject gameObject = new GameObject(gameEvent.name);
            gameObject.transform.parent = parent;
            GameEventRunner runner = gameObject.AddComponent<GameEventRunner>();
            runner._gameEvent = gameEvent;
            runner.ExecuteEvent();
        }
    }
}
