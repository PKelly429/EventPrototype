using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameEventSystem
{
    [System.Serializable]
    public class GameEventNode
    {
        [SerializeField] private string _guid;
        [SerializeField] private Rect _position;

        public string TypeName;

        public string Id => _guid;
        public Rect Position => _position;

        public GameEventNode()
        {
            GenerateGUID();
        }

        public void SetPosition(Rect position)
        {
            _position = position;
        }

        private void GenerateGUID()
        {
            _guid = System.Guid.NewGuid().ToString();
        }
    }
}
