using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Utility : MonoBehaviour {
    static public IEnumerator CoroutineTask(Action func, float seconds=0) {
        yield return new WaitForSeconds(seconds);
        func();
    }

    public class WaitForRead : IEnumerator {
        int count = 0;
        public object Current {
            get {
                count ++;
                return null;
            }
        }

        public bool MoveNext() {
            return count < 1000;
        }

        public void Reset() {
            throw new NotImplementedException();
        }
    }
}
