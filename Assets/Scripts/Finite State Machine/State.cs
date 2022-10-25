using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameObjectState {
    public class State {
        public delegate void ChangeEvent();
        public delegate void ChangeEventWithState(State state = null);
        string stateName;
        public string stateTag { get; private set; } = null;

        public State(string stateName) {
            this.stateName = stateName;
        }
        public State(string stateName, string stateTag) {
            this.stateName = stateName;
            this.stateTag = stateTag;
        }

        public ChangeEventWithState OnActive;
        public ChangeEvent OnStay;
        public ChangeEvent OnStayFixed;
        public ChangeEventWithState OnInactive;

        public override string ToString() {
            return stateName;
        }
        /* 
            Usage Example >>

            State jumpState = new State("Jump");
            jumpState.OnActive += () => {
                playerGameObject.ownRigidbody.addForce(Vector3.up * jumpPower);
            };
        */
    }
}
