using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameEventSystem.Editor
{
    public class InspectorView : ScrollView
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }

        private GameEventNodeView _selectedNode;

        private VisualElement _contentContainer;

        public InspectorView()
        {
            _contentContainer = this.Q<VisualElement>("unity-content-container");
        }

        public void UpdateSelection(GameEventNodeView nodeView)
        {
            _contentContainer.Clear();
            
            _selectedNode = nodeView;
            if (nodeView == null || nodeView.Node == null) return;

            nodeView.Node.DrawInspector(_contentContainer);
        }

        public void HandleNodeUnselected(GameEventNodeView nodeView)
        {
            if (_selectedNode == nodeView)
            {
                UpdateSelection(null);
            }
        }

        public void Redraw()
        {
            UpdateSelection(_selectedNode);
        }
    }
}
