using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace GameEventSystem
{
    [Serializable]
    public class StringParameterBuilder : IBindable
    {
        [TextArea] public string text;
        public StringParameter[] parameters;
        private object[] values;

        [SerializeField] private bool needsRepaint;
        
        public string GetValue()
        {
            if (values == null)
            {
                values = new object[parameters.Length];
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                values[i] = parameters[i].GetValue();
            }

            return string.Format(text, values);
        }
        
        public void Bind(IBlackboard blackboard)
        {
            if (parameters != null)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    parameters[i].Bind(blackboard);
                }
            }
        }

        public void HandleTextUpdated()
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }
            
            Regex rx = new Regex("\\{.\\}");
            string[] words = rx.Split(text);
            if (words.Length == 0)
            {
                return;
            }

            int variables = words.Length - 1;
            if (parameters == null || parameters.Length != variables)
            {
                var old = parameters?.ToList();
                parameters = new StringParameter[variables];
                if (old != null)
                {
                    for (int i = 0; i < Mathf.Min(old.Count, parameters.Length); i++)
                    {
                        parameters[i] = old[i];
                    }
                }

                needsRepaint = true;
            }
            else
            {
                needsRepaint = false;
            }
        }
        
        public string ValidateValue()
        {
            values = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                values[i] = parameters[i].GetName();
            }

            return string.Format(text, values);
        }
    }
}
