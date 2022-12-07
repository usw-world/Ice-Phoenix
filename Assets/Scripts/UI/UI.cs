using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class UI : MonoBehaviour {
    [SerializeField] protected GameObject canvas;

    public virtual bool isActive {
        get {
            if(canvas != null) return canvas.activeInHierarchy;
            return gameObject.activeInHierarchy;
        }
    }

    protected GameObject initialFrame;

    protected virtual void Awake() {}

    public virtual void OnActive() {
        if(canvas != null) { canvas.SetActive(true); } 
        else { this.gameObject.SetActive(true); }
        activeEvent.Invoke();
    }
    public virtual void OnInactive() {
        if(canvas != null) { canvas.SetActive(false); }
        else { this.gameObject.SetActive(false); }
        inactiveEvent.Invoke();
    }
    public virtual void KeyPressEvent() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            UIManager.instance.CloseUI();
        }
    }
    
    [Serializable]
    protected class UIActiveEvent : UnityEngine.Events.UnityEvent {}

    [FormerlySerializedAs("Active Event")]
    [SerializeField]
    protected UIActiveEvent activeEvent;
    [FormerlySerializedAs("Inactive Event")]
    [SerializeField]
    protected UIActiveEvent inactiveEvent;
}
