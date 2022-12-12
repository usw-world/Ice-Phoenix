using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    static public GameManager instance { get; private set; }

    ServerConnector serverConnector;
    public GameData gameData { get; private set; }

    public void Awake() {
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
                targetName = "09 Unsafe-04";
                break;
            case SceneList.Test:
                targetName = "_ability_test_scene";
                break;
        }
        if(targetName != null)
            UnityEngine.SceneManagement.SceneManager.LoadScene(targetName);
    }
    public void SetAdaptations(int[] next) {
        if(gameData != null) gameData.adaptation = next;
        else Debug.LogWarning("Can't read game data. If your are not during debugging, Check connecting state.");
        SynchronizeData();
    }
    public void SetRate(int next, int nextGauge) {
        if(gameData != null) gameData.rate = next;
        if(gameData != null) gameData.rateGauge = nextGauge;
        else Debug.LogWarning("Can't read game data. If your are not during debugging, Check connecting state.");
        SynchronizeData();
    }
    public void SynchronizeData() {
        string payload = JsonUtility.ToJson(gameData);
        StartCoroutine(serverConnector.SynchronizeData(payload, () => {
            print("data sended!");
        }));
    }
    public class GameData {
        public string userKey;
        public int rate;
        public int rateGauge;
        public int sceneNo;
        public int clearCount;
        public int[] adaptation;
        
        public GameData(string userKey, int rate, int rateGauge, int sceneNo, int clearCount, int[] adaptation) {
            this.userKey = userKey;
            this.rate = rate;
            this.rateGauge = rateGauge;
            this.sceneNo = sceneNo;
            this.clearCount = clearCount;
            this.adaptation = adaptation;
        }
    }
}
