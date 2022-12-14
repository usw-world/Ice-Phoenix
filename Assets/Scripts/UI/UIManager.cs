using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {
    static public UIManager instance { get; private set; }
    public UI activingUI { get; private set; } = null;

    [SerializeField] public EscapeMenuUI escapeMenuUI;
    [SerializeField] public StatusUI playerStatusUI;
    [SerializeField] public ScreenUI screenUI;
    [SerializeField] public AbilityChoicesUI abilityChoicesUI;
    [SerializeField] public DamageTextGenerator damageLog;
    [SerializeField] public DialogComponent dialogUI;

    [SerializeField] public SoundPlayer soundPlayer;

    [SerializeField] UnityEngine.UI.Image fadeInOutImage;

    public void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            // GameManager.instance.destroyObjectsOnGameOver.Add(this.gameObject);
        }
        else
            Destroy(this.gameObject);

        if(playerStatusUI == null) {
            Debug.LogWarning("There is any 'Player Status UI'.");
        }
    }
    public void CloseUI() {
        if(activingUI != null) activingUI.OnInactive();
        InputManager.instance.SetInputState(InputManager.instance.playState);
    }
    public void CloseUI(UI targetUI) {
        CloseUI();
        activingUI = targetUI;
    }
    public void OpenUI(UI targetUI) {
        InputManager.instance.SetInputState(InputManager.instance.menuState);
        if(activingUI != null && activingUI.isActive) {
            activingUI.OnInactive();
        }
        activingUI = targetUI;
        activingUI.OnActive();
    }
    public void TogglePlayerStatusUI() {
        if(!playerStatusUI.isActive) {
            OpenUI(playerStatusUI);
        } else {
            CloseUI(playerStatusUI);
        }
    }
    public void ZoomInCamera() {
        Player.playerInstance.ZoomInCamera();
    }
    public void ZoomOutCamera () {
        Player.playerInstance.ZoomOutCamera();
    }
    
    public void ChangeToPlayState() {
        InputManager.instance.ChangeToPlayState();
    }
    public void ChangeToMenuState() {
        InputManager.instance.ChangeToMenuState();
    }
    public void ChangeToDialogState() {
        InputManager.instance.ChangeToDialogState();
    }

    public void FadeIn(System.Action callback) {
        StartCoroutine(FadeInCoroutine(callback));
    }
    private IEnumerator FadeInCoroutine(System.Action callback) {
        float offset = 0;
        Color start = fadeInOutImage.color;
        while(offset < 1) {
            offset += Time.deltaTime / 2;
            fadeInOutImage.color = new Color(start.r, start.g, start.b, 1-offset);
            yield return null;
        }
        fadeInOutImage.gameObject.SetActive(false);
        if(callback != null)
            callback();
    }
    public void FadeOut(System.Action callback) {
        StartCoroutine(FadeOutCoroutine(callback));
    }
    public void FadeOut(System.Action callback, float second) {
        StartCoroutine(FadeOutCoroutine(callback, second));
    }
    private IEnumerator FadeOutCoroutine(System.Action callback, float second=2) {
        float offset = 0;
        Color start = fadeInOutImage.color;
        fadeInOutImage.gameObject.SetActive(true);
        while(offset < 1) {
            offset += Time.deltaTime / second;
            fadeInOutImage.color = new Color(start.r, start.g, start.b, offset);
            yield return null;
        }
        if(callback != null)
            callback();
    }
}
