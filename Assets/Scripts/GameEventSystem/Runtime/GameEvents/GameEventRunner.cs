using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    public class GameEventRunner : MonoBehaviour
    {
        [SerializeField] private GameEvent _gameEvent; // used to initialize an event on start

        [NonSerialized] private GameEvent _runtimeEvent;

        public GameEvent GameEvent => _runtimeEvent;

        public void OnEnable()
        {
            if (Application.isPlaying)
            {
                SetupEvent();
            }
        }

        private void SetupEvent()
        {
            if (_gameEvent == null) return;

            _runtimeEvent = _gameEvent.Clone();
            
            _runtimeEvent.Setup();
        }
        
        public static void StartEvent(GameEvent gameEvent, Transform parent = null)
        {
            GameObject gameObject = new GameObject(gameEvent.name);
            gameObject.transform.parent = parent;
            GameEventRunner runner = gameObject.AddComponent<GameEventRunner>();
            runner._gameEvent = gameEvent;
            runner.SetupEvent();
        }
    }
}
