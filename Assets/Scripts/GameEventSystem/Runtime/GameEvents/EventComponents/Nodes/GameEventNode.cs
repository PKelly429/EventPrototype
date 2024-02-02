using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameEventSystem
{
    [System.Serializable]
    public class GameEventNode
    {
        public List<GameEventConnection> Inputs;
        public List<GameEventConnection> Outputs;
        
        [SerializeField] private string _guid;
        [SerializeField] private Rect _position;

        public string TypeName;

        public string Id => _guid;
        public Rect Position => _position;

        public GameEventNode()
        {
            GenerateGUID();
            Inputs = new List<GameEventConnection>();
            Outputs = new List<GameEventConnection>();
        }
        
        public virtual bool Execute()
        {
            return true;
        }

        public void SetPosition(Rect position)
        {
            _position = position;
        }
        
        public void AddConnection(GameEventConnection connection)
        {
            if (connection.InputNodeId.Equals(Id))
            {
                Outputs.Add(connection);
            }
            else if (connection.OutputNodeId.Equals(Id))
            {
                Inputs.Add(connection);
            }
        }

        public void RemoveConnection(GameEventConnection connection)
        {
            if (connection.InputNodeId.Equals(Id))
            {
                for(int i=Outputs.Count-1; i>=0 ;i--)
                {
                    if (Outputs[i].Id.Equals(connection.Id))
                    {
                        Outputs.RemoveAt(i);
                        return;
                    }
                }
            }
            else if (connection.OutputNodeId.Equals(Id))
            {
                for(int i=Inputs.Count-1; i>=0 ;i--)
                {
                    if (Inputs[i].Id.Equals(connection.Id))
                    {
                        Inputs.RemoveAt(i);
                        return;
                    }
                }
            }
        }

        private void GenerateGUID()
        {
            _guid = System.Guid.NewGuid().ToString();
        }
    }
}
