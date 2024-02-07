using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ModalWindowOption : Selectable, IPointerClickHandler, ISubmitHandler
{
    public TMP_Text optionText;

    private Action optionClickedCallback;

    public void SetChoice(string text, bool active, Action callback)
    {
        optionText.text = text;
        interactable = active;
        optionClickedCallback = callback;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        optionClickedCallback?.Invoke();
        Press();
    }

    #region Button
        [Serializable]
        public class ButtonClickedEvent : UnityEvent {}

        // Event delegates triggered on click.
        [SerializeField]
        private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();
        
        public ButtonClickedEvent onClick
        {
            get { return m_OnClick; }
            set { m_OnClick = value; }
        }

        private void Press()
        {
            if (!IsActive() || !IsInteractable())
                return;

            UISystemProfilerApi.AddMarker("Button.onClick", this);
            m_OnClick.Invoke();
        }
        
        public virtual void OnSubmit(BaseEventData eventData)
        {
            Press();
            
            if (!IsActive() || !IsInteractable())
                return;

            DoStateTransition(SelectionState.Pressed, false);
            StartCoroutine(OnFinishSubmit());
        }

        private IEnumerator OnFinishSubmit()
        {
            var fadeTime = colors.fadeDuration;
            var elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            DoStateTransition(currentSelectionState, false);
        }
    

    #endregion
}
