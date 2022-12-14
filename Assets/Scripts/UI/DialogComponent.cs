using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine.UI;
using TMPro;

public class DialogComponent : MonoBehaviour {
    [SerializeField] public GameObject frame;
    [SerializeField] TextMeshProUGUI speackerTmp;
    [SerializeField] TextMeshProUGUI scriptTmp;
    
    public void ClearDialog() {
        speackerTmp.text = "";
        scriptTmp.text = "";
    }
    public void ReadAll(DialogSet dialogs, System.Action callBack) {
        StartCoroutine(ReadAllCoroutine(dialogs, callBack));
    }
    public IEnumerator ReadAllCoroutine(DialogSet dialogs, System.Action callback) {
        dialogs.Reset();
        InputManager.instance.ChangeToDialogState();
        
        while(dialogs.Next()) {
            DialogSet.Dialog script = dialogs.GetCurrent();
            speackerTmp.text = script.speacker;
            scriptTmp.text = script.script;
            yield return new Utility.WaitReturnEnumerator();
        }
        callback();
        yield return new WaitForSeconds(2);
        ClearDialog();
    }
}
