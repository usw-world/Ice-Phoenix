using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class UI : MonoBehaviour {
    [SerializeField] GameObject currentFrame;
    [SerializeField] GameObject[] ownFrame;

    GameObject initialFrame;

    protected virtual void Awake() {
        if(initialFrame == null && ownFrame.Length>0)
            initialFrame = ownFrame[0];
        if(currentFrame == null && ownFrame.Length>0)
            currentFrame = ownFrame[0];
        if(currentFrame == null)
            Debug.LogError($"UI Object %{this.gameObject.name}% has not any canvas. Please define 'changeFrame'.");

        foreach(GameObject f in ownFrame)  {
            f.SetActive(false);
        }
        if(currentFrame != null)
            currentFrame.gameObject.SetActive(true);
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
        currentFrame.gameObject.SetActive(false);
        currentFrame = nextFrame;
        currentFrame.gameObject.SetActive(true);
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
