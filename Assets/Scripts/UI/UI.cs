using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class UI : MonoBehaviour {
    [SerializeField] GameObject changeFrame;
    [SerializeField] GameObject[] ownFrame;

    GameObject initialFrame;

    protected virtual void Awake() {
        if(initialFrame == null && ownFrame.Length>0)
            initialFrame = ownFrame[0];
        if(changeFrame == null && ownFrame.Length>0)
            changeFrame = ownFrame[0];
        if(changeFrame == null)
            Debug.LogError($"UI Object %{this.gameObject.name}% has not any canvas. Please define 'changeFrame'.");

        foreach(GameObject f in ownFrame)  {
            f.SetActive(false);
        }
        if(changeFrame != null)
            changeFrame.gameObject.SetActive(true);
    }

    public void OnActive() {
        this.gameObject.SetActive(true);
        activeEvent.Invoke();
    }
    public void OnInactive() {
        this.gameObject.SetActive(false);
        inactiveEvent.Invoke();
    }
    public void ChangeFrame(GameObject nextFrame) {
        changeFrame.gameObject.SetActive(false);
        changeFrame = nextFrame;
        changeFrame.gameObject.SetActive(true);
    }
    
    [Serializable]
    class UIActiveEvent : UnityEngine.Events.UnityEvent {}

    [FormerlySerializedAs("Active Event")]
    [SerializeField]
    UIActiveEvent activeEvent;
    [FormerlySerializedAs("Inactive Event")]
    [SerializeField]
    UIActiveEvent inactiveEvent;
}
