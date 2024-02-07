using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

namespace GameEventSystem
{
    public class EventManager : MonoBehaviour
    {
        //Settings
        public float AverageEmergentEventDelay = 300;
        
        public List<GameEvent> gameEvents = new List<GameEvent>();

        private static EventManager _eventManager;
        private List<GameEvent> _emergentEvents = new List<GameEvent>();
        private HashSet<string> _triggeredEventIds = new HashSet<string>(); // used to test if an event has previously been triggered
        private HashSet<string> _completedEventIds = new HashSet<string>(); // events that have finished and cannot repeat
        private IEnumerator _eventScheduler;

        public static void RegisterEmergentEvent(GameEvent gameEvent)
        {
            if (_eventManager == null) return;
            _eventManager._emergentEvents.Add(gameEvent);
        }
        
        public static void RemoveEmergentEvent(GameEvent gameEvent)
        {
            if (_eventManager == null) return;
            _eventManager._emergentEvents.Remove(gameEvent);
        }

        public static bool HasEventTriggered(GameEvent gameEvent)
        {
            if (_eventManager == null) return false;
            return _eventManager._triggeredEventIds.Contains(gameEvent.UniqueId);
        }
        
        public static bool IsEventComplete(GameEvent gameEvent)
        {
            if (_eventManager == null) return false;
            return _eventManager._completedEventIds.Contains(gameEvent.UniqueId);
        }
        
        #region Event Scheduler
        public void StartScheduler()
        {
            _eventScheduler = EventScheduler();
        }
        
        private IEnumerator EventScheduler()
        {
            float time = 0f;
            float waitTime = 0f;
            float variance = AverageEmergentEventDelay * 0.3f;
            while (true)
            {
                time = 0f;
                waitTime = Random.Range(AverageEmergentEventDelay - variance, AverageEmergentEventDelay + variance);
                
                while(time < waitTime)
                {
                    time += Time.deltaTime;
                    yield return null;
                }

                IEnumerator fireEvent = FireEmergentEvent();
                while (fireEvent.MoveNext())
                {
                    yield return null;
                }
            }
        }


        private List<GameEvent> _eventsToTest = new List<GameEvent>();
        private IEnumerator FireEmergentEvent()
        {
            _eventsToTest.Clear();
            _eventsToTest.AddRange(_emergentEvents);
            while (true)
            {
                if (_eventsToTest == null || _eventsToTest.Count < 1) break;
                int indexToText = Random.Range(0, _eventsToTest.Count);
                if (_eventsToTest[indexToText].CheckConditions())
                {
                    _eventsToTest[indexToText].FireEvent();
                    break;
                }
                
                _eventsToTest.RemoveAt(indexToText);
                yield return null;
            }
        }
        #endregion
        
        #region Unity Events
        private void Start()
        {
            _eventManager = this;
            StartScheduler();
            LoadEvents();
        }
        private void Update()
        {
            try
            {
                _eventScheduler.MoveNext();
            }
            catch (Exception e)
            {
                #if DEBUG
                Debug.LogException(e);
                Debug.LogError("Caught exception in event scheduler, restarting");
                #endif
                _eventScheduler = EventScheduler();
            }
        }

        #endregion
        
        #region Event Initialization
        public const string GameEventAssetLabel = "GameEvent";
        void LoadEvents()
        {
            _eventManager = this;
            StartScheduler();
            
            var loadHandle = Addressables.LoadAssetsAsync<GameEvent>(GameEventAssetLabel, LoadGameEvent);
            loadHandle.Completed += AllGameEventsLoaded;
        }

        private void AllGameEventsLoaded(AsyncOperationHandle<IList<GameEvent>> obj)
        {
            
        }

        private void LoadGameEvent(GameEvent gameEvent)
        {
            GameEventRunner.StartEvent(gameEvent, transform);
        }
        #endregion
        
    }
}

