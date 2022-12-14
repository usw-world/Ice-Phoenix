using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;
using TMPro;

// UnityWebRequest.Get example

// Access a website and use UnityWebRequest.Get to download a page.
// Also try to download a non-existing page. Display the error.

public class ServerConnector : MonoBehaviour
{
    const string SERVER_HOST = "http://34.64.81.199:2022/";
    // const string SERVER_HOST = "localhost:2022/";
    public GameObject logMessageBox;

    [SerializeField]
    public class Response {
        public int result;
        public string userKey;
        public string message;
    }
    public IEnumerator GetNewUserKey(Action<string> Callback) {
        WWWForm www = new WWWForm();
        // form.AddField("key", "value");
        using (UnityWebRequest webRequest = UnityWebRequest.Post(SERVER_HOST + "create-user" , www)) {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
                Debug.Log(webRequest.error);
            else {
                string json = webRequest.downloadHandler.text;
                Response res = JsonUtility.FromJson<Response>(json);
                if(res.result == 200) {
                    Callback(res.userKey);
                } else {
                    TextMeshProUGUI text = logMessageBox.GetComponentInChildren<TextMeshProUGUI>();
                    text.text = res.message;
                    logMessageBox.SetActive(true);
                }
            }
            webRequest.Dispose();
        }
    }
    public IEnumerator LoadGameData(string userKey, Action Callback) {
        WWWForm www = new WWWForm();
        www.AddField("userKey", userKey);
        using (UnityWebRequest webRequest = UnityWebRequest.Post(SERVER_HOST + "get-data", www)) {
            yield return webRequest.SendWebRequest();
            string json = webRequest.downloadHandler.text;
            if(webRequest.result == UnityWebRequest.Result.Success) {
                GameManager.GameData gameData = JsonUtility.FromJson<GameManager.GameData>(json);
                GameManager.instance.SetGameData(gameData);
                print(json);
                Callback();
            } else {
                print(json);
                Response res = JsonUtility.FromJson<Response>(json);
                TextMeshProUGUI text = logMessageBox.GetComponentInChildren<TextMeshProUGUI>();
                text.text = res.message;
                logMessageBox.SetActive(true);
                // foreach(string keys in webRequest.GetResponseHeaders().Keys) {
                //     print(keys + " : " + webRequest.GetResponseHeaders()[keys]);
                // }
            }
        }
    }
    class RequestForLoadData {
        string userKey;
        public RequestForLoadData(string userKey) {
            this.userKey = userKey;
        }
    }
    
    System.Collections.Generic.Queue<SynchroData> synchronizationQueue = new System.Collections.Generic.Queue<SynchroData>();
    bool isSynchronizing = false;
    public IEnumerator SynchronizeData(string dataJson, Action Callback) {
        if(isSynchronizing) {
            synchronizationQueue.Enqueue(new SynchroData(dataJson, Callback));
        } else {
            WWWForm www = new WWWForm();
            www.AddField("dataJson", dataJson);
            using (UnityWebRequest webRequest = UnityWebRequest.Post(SERVER_HOST + "sync-data", www)) {
                yield return webRequest.SendWebRequest();
                string json = webRequest.downloadHandler.text;
                Response res = JsonUtility.FromJson<Response>(json);
                if(res.result == 200) {
                    Callback();
                } else {
                    Debug.LogWarning("Some error happen in sychronize data.");
                }
            }
            if(synchronizationQueue.Count > 0) {
                SynchroData next = synchronizationQueue.Dequeue();
                StartCoroutine(SynchronizeData(next.data, next.callback));
            } else {
                isSynchronizing = false;
            }
        }
    }
    struct SynchroData {
        public string data;
        public Action callback;
        public SynchroData(string data, Action callback) {
            this.data = data;
            this.callback = callback;
        }
    }
}