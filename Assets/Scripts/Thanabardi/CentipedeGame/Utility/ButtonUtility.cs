using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Thanabardi.CentipedeGame.Utility.UI
{
    public class ButtonUtility : MonoBehaviour, IPointerEnterHandler, ISelectHandler
    {
        public Button Button;

        public event Action OnPointerClickButton;
        public event Action OnPointerEnterButton;
        public event Action OnSelectButton;

        private void OnEnable()
        {
            Button.onClick.AddListener(OnClickHandler);
        }

        private void OnDisable()
        {
            Button.onClick.RemoveListener(OnClickHandler);
        }

        public void OnClickHandler()
        {
            OnPointerClickButton?.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Button.Select();
            OnPointerEnterButton?.Invoke();
        }

        public void OnSelect(BaseEventData eventData)
        {
            OnSelectButton?.Invoke();
        }
    }
}