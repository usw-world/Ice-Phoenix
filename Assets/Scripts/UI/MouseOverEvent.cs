using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class MouseOverEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [System.Serializable]
    class MouseEvent : UnityEvent {}

    [SerializeField]
    MouseEvent mouseEnterEvents;
    [SerializeField]
    MouseEvent mouseExitEvents;

    public void OnPointerEnter(PointerEventData eventData) {
        mouseEnterEvents.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData) {
        mouseExitEvents.Invoke();
    }
}
