using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    public class NodeRunner : MonoBehaviour
    {
        public static int SpreadOverFrames = 5;
        
        private List<GameEventNode> _runningNodes = new List<GameEventNode>();

        private IEnumerator _eventScheduler;
        private IEnumerator _nodeRunner;
        
        private bool _isSafeToEdit;
        private List<GameEventNode> _nodeRunnerAdditions = new List<GameEventNode>();
        private List<GameEventNode> _nodeRunnerRemovals = new List<GameEventNode>();

        private void Update()
        {
            try
            {
                if(_nodeRunner == null) _nodeRunner = Runner();
                _nodeRunner.MoveNext();
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.LogException(e);
                Debug.LogError("Caught exception in node runner, restarting");
#endif
                _nodeRunner = Runner();
            }
        }

        private IEnumerator Runner()
        {
            int updated = 0;
            int toUpdate =_runningNodes.Count/SpreadOverFrames;

            int frameCount = 0;
            while (true)
            {
                _isSafeToEdit = true;
                ProcessAddAndRemovals();
                _isSafeToEdit = false;

                if (_runningNodes.Count < 1)
                {
                    frameCount++;
                    yield return null;
                    continue;
                }

                for (var i = 0; i < _runningNodes.Count; i++)
                {
                    updated++;
                    if (updated >= toUpdate)
                    {
                        frameCount++;
                        yield return null;
                        
                        updated = 0;
                        toUpdate =_runningNodes.Count/SpreadOverFrames;
                    }

                    try
                    {
                        _runningNodes[i].Execute();
                    }
                    catch (Exception e)
                    {
                        #if DEBUG
                        Debug.LogException(e);
                        #endif
                    }
                }

                frameCount++;
                yield return null;
            }
        }

        public void AddNodeToRunningNodes(GameEventNode node)
        {
            if (_isSafeToEdit)
            {
                _runningNodes.Add(node);
                return;
            }
            
            _nodeRunnerAdditions.Add(node);
        }

        public void RemoveNodeFromRunningNodes(GameEventNode node)
        {
            if (_isSafeToEdit)
            {
                _runningNodes.Remove(node);
                return;
            }
            
            _nodeRunnerRemovals.Add(node);
        }

        private void ProcessAddAndRemovals()
        {
            foreach (var node in _nodeRunnerRemovals)
            {
                _runningNodes.Remove(node);
            }
            _nodeRunnerRemovals.Clear();
            
            foreach (var node in _nodeRunnerAdditions)
            {
                _runningNodes.Add(node);
            }
            _nodeRunnerAdditions.Clear();
        }
    }
}