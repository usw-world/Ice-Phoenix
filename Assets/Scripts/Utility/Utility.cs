using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Utility : MonoBehaviour {
    static public IEnumerator CoroutineTask(Action func, float seconds=0) {
        yield return new WaitForSeconds(seconds);
        func();
    }
}
