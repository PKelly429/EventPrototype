using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    [NodeInfo("Wait For Seconds", "FLOW")]
    [NodeDescription("Halt for time")]
    public class WaitForSecondsNode : EffectNode
    {
        [DisplayField] public float Seconds;
        [DisplayField] [ReadOnly] public float _timeRemaining;

        private float _fromTime;

        protected override void OnStart()
        {
            _fromTime = Time.time;
            base.OnStart();
        }

        protected override State OnUpdate()
        {
            if (CheckTime())
            {
                return State.Success;
            }
            
            return State.Running;
        }
        
        private bool CheckTime()
        {
            float timePassed = Time.time - _fromTime;
            _timeRemaining = Seconds - timePassed;
            if (_timeRemaining <= 0)
            {
                _timeRemaining = 0;
                return true;
            }
            
            return false;
        }
    }
}