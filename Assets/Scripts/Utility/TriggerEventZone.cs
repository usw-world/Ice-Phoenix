using UnityEngine;
using UnityEngine.Serialization;

public class TriggerEventZone : MonoBehaviour {
    bool hasplayed = false;

    [System.Serializable]
    public class TriggerEvent : UnityEngine.Events.UnityEvent {}
    [FormerlySerializedAs("Active Event")]
    [SerializeField]
    protected TriggerEvent triggerEnterEvent;

    void OnTriggerEnter2D(Collider2D other) {
        if(!hasplayed) {
            if(other.tag == "Player") {
                triggerEnterEvent.Invoke();
                hasplayed = true;
            }
        }
    }
}
