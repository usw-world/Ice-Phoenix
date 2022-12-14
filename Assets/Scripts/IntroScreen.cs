using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class IntroScreen : MonoBehaviour {
    string PRODUCT_DIR = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + "\\Ice Phoenix";
    string SAVE_FILE_NAME = "\\userinfo.csv";

    [SerializeField] GameObject confirmCreateSaveFrame;
    ServerConnector connector;

    [SerializeField] GameObject introStage;
    [SerializeField] AudioSource bgmAudioSource;

    void Awake() {
        connector = GameManager.instance.gameObject.GetComponent<ServerConnector>();
    }
    void Update() {
        if(Input.GetKeyDown(KeyCode.Return)) {
            StartGame();
        }
        if(Input.GetKeyDown(KeyCode.F3)) {
            GameManager.instance.ChangeScene(GameManager.SceneList.Safe01);
        }
    }
    public void StartGame() {
        if(CheckLocalUserKey()) { // There is save-data in local.
            LoadSaveData();
        } else {
            confirmCreateSaveFrame.SetActive(true);
        }
    }
    private bool CheckLocalUserKey() {
        if(File.Exists(PRODUCT_DIR + SAVE_FILE_NAME)) {
            return true;
        } else {
            return false;
        }
    }
    public void CreateSave() {
        confirmCreateSaveFrame.SetActive(false);
        StartCoroutine(connector.GetNewUserKey((string userKey) => {
            if(File.Exists(PRODUCT_DIR + SAVE_FILE_NAME))
                return;

            Directory.CreateDirectory(PRODUCT_DIR);
            FileInfo userinfo = new FileInfo(PRODUCT_DIR + SAVE_FILE_NAME);
            var fs = userinfo.Create();
            fs.Close();

            StreamWriter writer = new StreamWriter(PRODUCT_DIR + SAVE_FILE_NAME);
            System.DateTime now = System.DateTime.Now;
            writer.Write("user_key,register_date\n" + $"{userKey},{now.ToShortDateString()}");
            writer.Close();

            LoadSaveData();
        }));
    }
    public void LoadSaveData() {
        if(!File.Exists(PRODUCT_DIR + SAVE_FILE_NAME)) return;

        FileInfo userinfo = new FileInfo(PRODUCT_DIR + SAVE_FILE_NAME);
        StreamReader reader = new StreamReader(PRODUCT_DIR + SAVE_FILE_NAME);
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
            StartCoroutine(connector.LoadGameData(userKey, () => {
                introStage.GetComponent<Animation>().Play();
                bgmAudioSource.gameObject.GetComponent<SoundPlayer>().FadeOutSound();
                StartCoroutine(Utility.TimeoutTask(() => {
                    if(GameManager.instance.gameData.sceneNo == 0) {
                        GameManager.instance.ChangeScene(GameManager.SceneList.Tutorial);
                    } else {
                        GameManager.instance.ChangeScene(GameManager.SceneList.Test);
                    }
                }, 3));
            }));
        }
    }
}
