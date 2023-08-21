using UnityEngine;
using UnityEngine.Serialization;

public class TriggerEventZone : MonoBehaviour {
    bool hasPlayed = false;

    [System.Serializable]
    public class TriggerEvent : UnityEngine.Events.UnityEvent {}
    // [FormerlySerializedAs("Active Event")]
    [SerializeField] protected TriggerEvent triggerEnterEvent;

    void OnTriggerEnter2D(Collider2D other) {
        if(!hasPlayed) {
            if(other.tag == "Player") {
                triggerEnterEvent.Invoke();
                hasPlayed = true;
            }
        }
    }
}
