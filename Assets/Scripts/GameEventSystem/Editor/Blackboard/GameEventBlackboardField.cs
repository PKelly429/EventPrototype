using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameEventSystem.Editor
{
    public class GameEventBlackboardField : VisualElement
    {
        public Action<VisualElement, string> editTextRequested;
        
        private VisualElement m_ContentItem;
        private Pill m_Pill;
        private TextField m_TextField;
        private Label m_TypeLabel;
        
        /// <summary>
        ///   <para>The text of this BlackboardField.</para>
        /// </summary>
        /// <footer><a href="https://docs.unity3d.com/2021.3/Documentation/ScriptReference/30_search.html?q=Experimental.GraphView.BlackboardField-text">`BlackboardField.text` on docs.unity3d.com</a></footer>
        public string text
        {
            get => this.m_Pill.text;
            set => this.m_Pill.text = value;
        }

        /// <summary>
        ///   <para>The text that displays the data type of this BlackboardField.</para>
        /// </summary>
        /// <footer><a href="https://docs.unity3d.com/2021.3/Documentation/ScriptReference/30_search.html?q=Experimental.GraphView.BlackboardField-typeText">`BlackboardField.typeText` on docs.unity3d.com</a></footer>
        public string typeText
        {
            get => this.m_TypeLabel.text;
            set => this.m_TypeLabel.text = value;
        }
        
        public GameEventBlackboardField()
        {
            VisualElement visualElement = (VisualElement) (EditorGUIUtility.Load("UXML/GraphView/BlackboardField.uxml") as VisualTreeAsset).Instantiate();
            //this.AddStyleSheetPath(Blackboard.StyleSheetPath);
            visualElement.AddToClassList("mainContainer");
            visualElement.pickingMode = PickingMode.Ignore;
            this.m_ContentItem = visualElement.Q("contentItem", (string) null);
            this.m_Pill = visualElement.Q<Pill>("pill", (string) null);
            this.m_TypeLabel = visualElement.Q<Label>("typeLabel", (string) null);
            this.m_TextField = visualElement.Q<TextField>("textField", (string) null);
            this.m_TextField.style.display = (StyleEnum<DisplayStyle>) DisplayStyle.None;
            this.m_TextField.Q(TextInputBaseField<string>.textInputUssName, (string) null).RegisterCallback<FocusOutEvent>((EventCallback<FocusOutEvent>) (e => this.OnEditTextFinished()));
            this.Add(visualElement);
            this.RegisterCallback<MouseDownEvent>(new EventCallback<MouseDownEvent>(this.OnMouseDownEvent));

            this.ClearClassList();
            this.AddToClassList("blackboardField");
            this.text = text;
            this.typeText = typeText;
        }

        private void OnEditTextFinished()
        {
            this.m_ContentItem.visible = true;
            this.m_TextField.style.display = (StyleEnum<DisplayStyle>) DisplayStyle.None;
            if (!(this.text != this.m_TextField.text))
                return;
            if (this.editTextRequested != null)
                this.editTextRequested((VisualElement) this, this.m_TextField.text);
            else
                this.text = this.m_TextField.text;
        }

        private void OnMouseDownEvent(MouseDownEvent e)
        {
            if (e.clickCount != 2 || e.button != 0)
                return;
            this.OpenTextEditor();
            e.PreventDefault();
        }

        /// <summary>
        ///   <para>Opens a TextField to edit the text in a BlackboardField.</para>
        /// </summary>
        /// <footer><a href="https://docs.unity3d.com/2021.3/Documentation/ScriptReference/30_search.html?q=Experimental.GraphView.BlackboardField.OpenTextEditor">`BlackboardField.OpenTextEditor` on docs.unity3d.com</a></footer>
        public void OpenTextEditor()
        {
            this.m_TextField.SetValueWithoutNotify(this.text);
            this.m_TextField.style.display = (StyleEnum<DisplayStyle>) DisplayStyle.Flex;
            this.m_ContentItem.visible = false;
            this.m_TextField.Q(TextInputBaseField<string>.textInputUssName, (string) null).Focus();
            this.m_TextField.SelectAll();
        }
    }
}
