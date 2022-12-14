using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Utility : MonoBehaviour {
    static public IEnumerator TimeoutTask(Action func, float seconds=0) {
        yield return new WaitForSeconds(seconds);
        func();
    }
    
    public class WaitReturnEnumerator : IEnumerator {
        bool next = false;
        public object Current {
            get {
                next = Input.GetKeyDown(KeyCode.Return);
                return null;
            }
        }
        public void Reset() {}
        public bool MoveNext() {
            if(next) {
                next = false;
                return false;
            } else {
                return true;
            }
        }
    }
}
