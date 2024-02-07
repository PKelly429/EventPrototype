using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    
    [AddTypeMenu("Time Passed")]
    public class TimePassedConditionNode : AbstractCondition
    {
        public enum StartTime
        {
            GameStart,
            EventStart
        }

        [DisplayField] public StartTime startTime;
        [DisplayField] public float Seconds;
        [DisplayField] [ReadOnly] public float _timeRemaining;

        private float _fromTime;
        
        
        public override bool CheckCondition(GameEvent parentEvent)
        {
            switch (startTime)
            {
                case StartTime.EventStart:
                {
                    _fromTime = parentEvent.startTime;
                    break;
                }
                default:
                    _fromTime = 0;
                    break;
            }
            
            float timePassed = Time.time - _fromTime;
            _timeRemaining = Seconds - timePassed;
            if (_timeRemaining <= 0)
            {
                _timeRemaining = 0;
                return true;
            }
            else
            {
                return false;
            }
        }
        
    }
}