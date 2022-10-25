using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectState;

namespace GameObjectState {
    public class StateMachine : MonoBehaviour {
        public State currentState { get; private set; }

        private void Start() {
            if(currentState == null) {
                Debug.LogError($"StateMachine on '{gameObject.name}' has not any 'currentState'. Please set initial 'currentState' with SetInitialState(State state).");
            }
        }
        public void SetIntialState(State state) {
            if(currentState == null) {
                currentState = state;
            } else {
                Debug.LogError($"StateMachine on '{gameObject.name}' already has initial 'currentState'.");
            }
        }
        public void ChangeState(State nextState, bool intoSelf=true) { 
            if(!intoSelf && nextState==currentState) return;
            if(currentState == null) {
                Debug.LogError($"StateMachine on '{gameObject.name}' has not any 'currentState'. Please set initial 'currentState' with SetInitialState(State state).");
            } else {
                if(currentState.OnInactive != null) currentState.OnInactive(nextState);
                if(nextState.OnActive != null) nextState.OnActive(currentState);
                currentState = nextState;
            }
        }
        public bool Compare(State target) {
            return currentState == target;
        }
        private void Update() {
            if(currentState != null && currentState.OnStay != null) {
                currentState.OnStay();
            }
        }
        private void FixedUpdate() {
            if(currentState != null && currentState.OnStayFixed != null) {
                currentState.OnStayFixed();
            }
        }
    }
}
