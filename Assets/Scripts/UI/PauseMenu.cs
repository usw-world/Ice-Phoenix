using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

    public static bool isPause = false;

    public GameObject pausePanel;
    public GameObject pauseCanvas;
    public GameObject SettingMenu;
    public GameObject GotoMain;
    public GameObject ExitMenu;

    void Start()
    {
        isPause = false;
        Time.timeScale = 1f;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPause)
            {
                Resume();
            } else {
                Pause();
            }
        }        
    }

    public void Resume()
    {
        Time.timeScale = 1f;        
        pauseCanvas.SetActive(false);
        SettingMenu.SetActive(false);
        // GotoMain.SetActive(false);
        // ExitMenu.SetActive(false);
        isPause = false;
    }

    public void Pause()
    {
        Time.timeScale = 0f;        
        pauseCanvas.SetActive(true);
        isPause = true;
    }

    public void OnClickSettings()
    {
        pauseCanvas.SetActive(false);
        SettingMenu.SetActive(true);
    }

    public void OnClickSettingsBack()
    {
        SettingMenu.SetActive(false);
        pauseCanvas.SetActive(true);
    }
    public void OnClickExitGame()
    {
        pauseCanvas.SetActive(false);
        ExitMenu.SetActive(true);
    }
    public void OnClickExit_yes()
    {
        // UnityEditor.EditorApplication.isPlaying = false;
    }

    public void OnClickExit_CanCel()
    {
        ExitMenu.SetActive(false);
        pauseCanvas.SetActive(true);
    }

    public void OnClickGotoMain()
    {
        pauseCanvas.SetActive(false);
        GotoMain.SetActive(true);
    }
    public void OnClickMain_yes()
    {
        GotoMain.SetActive(false);
        SceneManager.LoadScene("MenuScene");
    }
    public void OnClickMain_CanCel()
    {
        GotoMain.SetActive(false);
        pauseCanvas.SetActive(true);
    }
}
