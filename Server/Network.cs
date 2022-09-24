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

public class Network : MonoBehaviour
{
    int CurrentUserNum = 1;
    string registerName = "lll";

    [SerializeField]
    public class UserData
    {
        public int id;
        public string name;
    }

    void Start()
    {
        // A correct website page.
        StartCoroutine(SelectUserData());
        StartCoroutine(InsertUserData());
    }

    IEnumerator SelectUserData() // ���� ������ �� ���� ������ id�� ã�� ������ �˻�. ���� �˻��� �𸣰���.
    {
        WWWForm form = new WWWForm();
        form.AddField("CurrentUserNum", CurrentUserNum); // CurrentUserNum�� �˻��ϰ��� �ϴ� DB�� �ִ� id ��ȣ. �˻� ���� �߰��ص� ��
        using (UnityWebRequest webRequest = UnityWebRequest.Post("http://localhost:3301/selectUser" , form))
        {
            yield return webRequest.Send();

            if (webRequest.isNetworkError)
                Debug.Log(webRequest.error);
            else
            {
                UserData userdata = JsonUtility.FromJson<UserData>(webRequest.downloadHandler.text);

                Debug.Log(userdata.id + " " + userdata.name);
            }
                
        }
    }

    IEnumerator InsertUserData()
    {
        WWWForm form = new WWWForm();
        form.AddField("registerName", registerName); // �ι�° ���ڴ� �߰��ϰ��� �ϴ� �̸�. �˻� ���� �߰��ص� ��
        using (UnityWebRequest webRequest = UnityWebRequest.Post("http://localhost:3301/insertUser", form))
        {
            yield return webRequest.Send();

            if (webRequest.isNetworkError)
                Debug.Log(webRequest.error);
            else
            {
                UserData userdata = JsonUtility.FromJson<UserData>(webRequest.downloadHandler.text);

                Debug.Log(userdata.id + " " + userdata.name);
            }

        }
    }
}