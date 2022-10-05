using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameObjectState {
    public class State {
        public delegate void voidEvent();
        string stateName;

        public State(string stateName) {
            this.stateName = stateName;
        }

        public voidEvent OnActive;
        public voidEvent OnStay;
        public voidEvent OnStayFixed;
        public voidEvent OnInactive;

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
