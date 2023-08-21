using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class IntroScreen : MonoBehaviour {
    string PRODUCT_DIR = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + "\\Ice Phoenix";
    string USER_KEY_FILENAME = "\\userinfo.csv";

    [SerializeField] GameObject confirmCreateSaveFrame;
    ServerConnector connector;

    [SerializeField] GameObject introStage;
    [SerializeField] AudioSource bgmAudioSource;

    public GameObject errorMessageBox;
    public GameObject localJoineBox;

    [SerializeField] GameObject __testObjs;

    void Start() {
        connector = GameManager.instance.gameObject.GetComponent<ServerConnector>();
    }
    void Update() {
        if(Input.GetKeyDown(KeyCode.Return)) {
            StartGame();
        }
        // if(Input.GetKeyDown(KeyCode.F3)) {
        //     Destroy(__testObjs);
        // }
    }
    public void StartGame() {
        /*
        if(CheckLocalUserKey()) { // If client has user-key to access own data in desktop.
            LoadSaveData();
        } else {
            confirmCreateSaveFrame.SetActive(true);
        }
        */
        if(!File.Exists(PRODUCT_DIR + USER_KEY_FILENAME)) {
            /* gameData = new GameManager.GameData(); */
        }
        introStage.GetComponent<Animation>().Play();
        bgmAudioSource.gameObject.GetComponent<SoundPlayer>().FadeOutSound();
        StartCoroutine(Utility.TimeoutTask(() => {
            StartWithLocalData();
        }, 3));
    }
    private bool CheckLocalUserKey() {
        if(File.Exists(PRODUCT_DIR + USER_KEY_FILENAME)) {
            return true;
        } else {
            return false;
        }
    }
    public void CreateSave() {
        confirmCreateSaveFrame.SetActive(false);
        StartCoroutine(connector.GetNewUserKey((string userKey) => {
            if(File.Exists(PRODUCT_DIR + USER_KEY_FILENAME))
                return;

            Directory.CreateDirectory(PRODUCT_DIR);
            FileInfo userinfo = new FileInfo(PRODUCT_DIR + USER_KEY_FILENAME);
            var fs = userinfo.Create();
            fs.Close();

            StreamWriter writer = new StreamWriter(PRODUCT_DIR + USER_KEY_FILENAME);
            System.DateTime now = System.DateTime.Now;
            writer.Write("user_key,register_date\n" + $"{userKey},{now.ToShortDateString()}");
            writer.Close();

            LoadSaveData();
        }, (string error) => {
            TextMeshProUGUI text = errorMessageBox.GetComponentInChildren<TextMeshProUGUI>();
            text.text = error;
            errorMessageBox.SetActive(true);
        }));
    }
    public void LoadSaveData() {
        if(!File.Exists(PRODUCT_DIR + USER_KEY_FILENAME))
            return;
        
        FileInfo userinfo = new FileInfo(PRODUCT_DIR + USER_KEY_FILENAME);
        StreamReader reader = new StreamReader(PRODUCT_DIR + USER_KEY_FILENAME);
        string[] heads = reader.ReadLine().Split(',');
        string[] datas = reader.ReadLine().Split(',');
        
        Dictionary<string, string> infoMap = new Dictionary<string, string>();
        for (int i=0; i<heads.Length; i++) {
            infoMap.Add(heads[i], datas[i]);
        }
        string userKey = infoMap["user_key"];
        if(userKey == null) {
            Debug.LogError("Found local user-key but any unknown error was happened.");
            return;
        } else {
            StartCoroutine(connector.LoadGameData(userKey, (bool successConnecting) => {
                if(successConnecting) {
                    introStage.GetComponent<Animation>().Play();
                    bgmAudioSource.gameObject.GetComponent<SoundPlayer>().FadeOutSound();
                    StartCoroutine(Utility.TimeoutTask(() => {
                        GameManager.instance.ChangeScene(GameManager.SceneList.Tutorial);
                    }, 3));
                } else {
                    localJoineBox.SetActive(true);
                }
            }));
        }
    }
    public void StartWithLocalData() {
        GameManager.instance.isLocalData = true;
        GameManager.instance.ChangeScene(GameManager.SceneList.Tutorial);
        GameManager.instance.LoadData();
    }
}
