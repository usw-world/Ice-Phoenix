using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System;

// UnityWebRequest.Get example

// Access a website and use UnityWebRequest.Get to download a page.
// Also try to download a non-existing page. Display the error.

public class ServerConnector : MonoBehaviour
{
    [SerializeField]
    public class Userinfo {
        public string id;
        public string name;
        public override string ToString() {
            return id + "/" + name;
        }
    }
    public void GetUser() {
        StartCoroutine(SelectUserData());
    }
    IEnumerator SelectUserData() {
        WWWForm form = new WWWForm();
        form.AddField("key", "value");
        using (UnityWebRequest webRequest = UnityWebRequest.Post("http://localhost:2022/get-user" , form)) {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
                Debug.Log(webRequest.error);
            else {
                var headers = webRequest.GetResponseHeaders();
                foreach(string k in headers.Keys) {
                    // print($"{k} : ${headers[k]}");
                }
                string json = webRequest.downloadHandler.text;
                print(json);
                Userinfo info = JsonUtility.FromJson<Userinfo>(json);
                print(info);
            }
            webRequest.Dispose();
        }
    }
}