using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    static public GameManager instance { get; private set; }

    ServerConnector serverConnector;
    public GameData gameData { get; private set; }

    public void Awake() {
        if(instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        serverConnector = GetComponent<ServerConnector>();

        DontDestroyOnLoad(this.gameObject);
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
    public enum SceneList {
        Test = 0,
    }
    public void ChangeScene(SceneList target) {
        string targetName = null;
        switch(target) {
            case SceneList.Test:
                targetName = "architecture_scene";
                break;
        }
        if(targetName != null)
            UnityEngine.SceneManagement.SceneManager.LoadScene(targetName);
    }
    public void SetAdaptations(int[] next) {
        if(gameData != null)
            gameData.adaptation = next;
        else 
            Debug.LogWarning("Can't read game data. If your are not during debugging, Check connecting state.");
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
        public int level;
        public int sceneNo;
        public int clearCount;
        public int[] adaptation;
        
        public GameData(string userKey, int level, int sceneNo, int clearCount, int[] adaptation) {
            this.userKey = userKey;
            this.level = level;
            this.sceneNo = sceneNo;
            this.clearCount = clearCount;
            this.adaptation = adaptation;
        }
    }
}
