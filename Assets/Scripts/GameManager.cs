using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    static public GameManager instance { get; private set; }

    public bool isLocalData = false;

    string PRODUCT_DIR = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + "\\Ice Phoenix";
    string USER_KEY_FILENAME = "\\userinfo.csv";

    public string SAVE_DATA_DIR = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + "\\Ice Phoenix\\SaveData";
    public string SAVE_DATA_FILENAME = "\\savedata.json";

    ServerConnector serverConnector;
    public GameData gameData { get; private set; }

    public List<GameObject> destroyObjectsOnGameOver = new List<GameObject>();

    public void Awake() {
        UnityEngine.Rendering.DebugManager.instance.enableRuntimeUI = false;

        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);

        serverConnector = GetComponent<ServerConnector>();
    }
    public void StopGame() {
        Time.timeScale = 0;
        InputManager.instance.SetInputState(InputManager.instance.menuState);
    }
    public void StartGame() {
        Time.timeScale = 1;
        InputManager.instance.SetInputState(InputManager.instance.playState);
    }
    public void SetGameData(GameData data) {
        if(gameData == null)
            gameData = data;
        else
            return;
    }
    [System.Serializable]
    public enum SceneList {
        Main = 0,
        Tutorial = 1,
        Safe01 = 2,
        Unsafe01 = 3,
        Safe02 = 4,
        Unsafe02 = 5,
        Safe03 = 6,
        Unsafe03 = 7,
        Safe04 = 8,
        Unsafe04 = 9,
        Lobby = 10,
        Test = 66,
    }
    public void ChangeScene(SceneList target) {
        string targetName = null;
        switch(target) {
            case SceneList.Main:
                targetName = "_ability_test_scene";
                break;
            case SceneList.Tutorial:
                targetName = "01 Start Scene";
                break;
            case SceneList.Safe01:
                targetName = "02 Safe-01";
                break;
            case SceneList.Unsafe01:
                targetName = "03 Unsafe-01";
                break;
            case SceneList.Safe02:
                targetName = "04 Safe-02";
                break;
            case SceneList.Unsafe02:
                targetName = "05 Unsafe-02";
                break;
            case SceneList.Safe03:
                targetName = "06 Safe-03";
                break;
            case SceneList.Unsafe03:
                targetName = "07 Unsafe-03";
                break;
            case SceneList.Safe04:
                targetName = "08 Safe-04";
                break;
            case SceneList.Unsafe04:
                targetName = "09 Boss";
                break;
            case SceneList.Lobby:
                targetName = "10 Lobby";
                break;
            case SceneList.Test:
                targetName = "_ability_test_scene";
                break;
        }
        if(targetName != null)
            UnityEngine.SceneManagement.SceneManager.LoadScene(targetName);
    }

    void Update() {
        __cheet();
    }
    /*  */
    public void __cheet() {
        if(Input.GetKey(KeyCode.LeftShift)) {
            if(Input.GetKeyDown(KeyCode.Alpha1)) ChangeScene(SceneList.Safe01);
            if(Input.GetKeyDown(KeyCode.Alpha2)) ChangeScene(SceneList.Safe02);
            if(Input.GetKeyDown(KeyCode.Alpha3)) ChangeScene(SceneList.Safe03);
            if(Input.GetKeyDown(KeyCode.Alpha4)) ChangeScene(SceneList.Safe04);
        }
    }
    /*  */
    public void GameClear() {
        GameOver(true);
    }
    public void GameOver(bool clear, float delay=2f) {
        gameData.clearCount++;
        SynchronizeData();
        StartCoroutine(Utility.TimeoutTask(() => {
            UIManager.instance.FadeOut(() => {
                foreach(GameObject gobj in destroyObjectsOnGameOver) {
                    Destroy(gobj);
                }
                LoadData();
                ChangeScene(SceneList.Lobby);
            }, 5);
        }, delay));
    }
    public void SetAdaptations(int[] next) {
        if(gameData != null)
            gameData.adaptation = next;
        else
            Debug.LogWarning("Can't read game data. If your are not during debugging, Check connecting state.");
        SynchronizeData();
    }
    public void SetRate(int next, int nextGauge) {
        if(gameData != null) gameData.rate = next;
        if(gameData != null) gameData.rateGauge = nextGauge;
        else Debug.LogWarning("Can't read game data. If your are not during debugging, Check connecting state.");
        SynchronizeData();
    }
    public void IncreaseClearCount() {
        if(gameData != null)
            gameData.clearCount ++;
    }
    public void SynchronizeData() {
        if(GameManager.instance.isLocalData) {
            print("Local save");
            if(!File.Exists(SAVE_DATA_DIR))
                Directory.CreateDirectory(SAVE_DATA_DIR);
            if(!File.Exists(SAVE_DATA_DIR + SAVE_DATA_FILENAME)) {
                FileStream fs = new FileInfo(SAVE_DATA_DIR + SAVE_DATA_FILENAME).Create();
                fs.Close();
            }
            string json = JsonUtility.ToJson(gameData);
            StreamWriter writer = new StreamWriter(SAVE_DATA_DIR + SAVE_DATA_FILENAME);
            writer.Write(json);
            writer.Close();
        } else {
            print("Remove save");
            string payload = JsonUtility.ToJson(gameData);
            StartCoroutine(serverConnector.SynchronizeData(payload, () => {
                print("data sended!");
            }));
        }

    }
    public class GameData {
        public string userKey;
        public int rate = 1;
        public int rateGauge = 0;
        public int sceneNo = 0;
        public int clearCount = 0;
        public int[] adaptation = new int[]{ 0, 0, 0, 0, 0 };
        
        public GameData() {}
        public GameData(string userKey, int rate, int rateGauge, int sceneNo, int clearCount, int[] adaptation) {
            this.userKey = userKey;
            this.rate = rate;
            this.rateGauge = rateGauge;
            this.sceneNo = sceneNo;
            this.clearCount = clearCount;
            this.adaptation = adaptation;
        }
    }
    public void LoadData() {
        if(isLocalData) {
            string saveDataDir = GameManager.instance.SAVE_DATA_DIR + GameManager.instance.SAVE_DATA_FILENAME;
            if(!File.Exists(saveDataDir)) {
                gameData = new GameData();
            } else {
                StreamReader reader = new StreamReader(GameManager.instance.SAVE_DATA_DIR + GameManager.instance.SAVE_DATA_FILENAME);
                string json = reader.ReadLine();
                gameData = JsonUtility.FromJson<GameManager.GameData>(json);
            }
        } else {
            if(!File.Exists(PRODUCT_DIR + USER_KEY_FILENAME)) return;
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
                StartCoroutine(serverConnector.LoadGameData(userKey, (bool successConnecting) => {
                    if(successConnecting) {
                        /* success */
                    } else {
                        /* fail */
                    }
                }));
            }
        }
    }
    public void GameQuit() {
        Application.Quit();
    }
}
