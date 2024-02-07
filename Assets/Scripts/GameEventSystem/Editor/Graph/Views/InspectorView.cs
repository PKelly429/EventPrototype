using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameEventSystem.Editor
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }

        private GameEventNodeView _selectedNode;
        private UnityEditor.Editor _editor;

        public InspectorView()
        {
        }

        public void UpdateSelection(GameEventNodeView nodeView)
        {
            Clear();
            UnityEngine.Object.DestroyImmediate(_editor);

            _selectedNode = nodeView;
            if (nodeView == null || nodeView.Node == null) return;

            nodeView.Node.DrawInspector(this);
            // _editor = UnityEditor.Editor.CreateEditor(nodeView.Node);
            // IMGUIContainer container = new IMGUIContainer(() =>
            // {
            //     if (_editor.target == null)
            //     {
            //         UpdateSelection(null);
            //         return;
            //     }
            //     _editor.OnInspectorGUI();
            // });
            //Add(container);
        }

        public void HandleNodeUnselected(GameEventNodeView nodeView)
        {
            if (_selectedNode == nodeView)
            {
                UpdateSelection(null);
            }
        }
    }
}
