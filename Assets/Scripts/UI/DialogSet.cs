using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogSet : MonoBehaviour {
    [System.Serializable]
    public class Dialog {
        public string speacker;
        [TextArea(4, 10)] public string script;
    }
    public int offset { get; private set; } = -1;
    [SerializeField] private Dialog[] dialogs;
    
    public bool Next() {
        offset ++;
        if(offset < dialogs.Length) return true;
        else return false;
    }
    public Dialog GetCurrent() {
        if(offset < dialogs.Length) {
            return dialogs[offset];
        } else {
            return null;
        }
    }
    public void Reset() {
        offset = -1;
    }
}
