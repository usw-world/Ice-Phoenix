using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class IntroScreen : MonoBehaviour {
    string PRODUCT_DIR = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + "\\Ice Phoenix";
        string SAVE_FILE_NAME = "\\userinfo.csv";

    [SerializeField] GameObject confirmCreateSaveFrame;
    ServerConnector connector;

    void Awake() {
        connector = GetComponent<ServerConnector>();
        connector.GetUser();
        // StartCoroutine(Foo());
    }
    IEnumerator Foo() {
        yield return new Utility.WaitForRead();
    }
    void Update() {
        if(Input.GetKeyDown(KeyCode.Return)) {
            if(!Directory.Exists(PRODUCT_DIR)) {
                confirmCreateSaveFrame.SetActive(true);
            }
        }
    }
    public void CreateSave() {
        Directory.CreateDirectory(PRODUCT_DIR);
        FileInfo userinfo = new FileInfo(PRODUCT_DIR + SAVE_FILE_NAME);
        var fs = userinfo.Create();
        fs.Close();

        StreamWriter writer = new StreamWriter(PRODUCT_DIR + SAVE_FILE_NAME);
        System.DateTime now = System.DateTime.Now;
        // writer.Write("user_key,register_date\n" + $"{},{now.ToShortDateString()}");
        writer.Close();
    }
    public string ReadUserkey() {

        return "Start spreading the news.";
    }
}
