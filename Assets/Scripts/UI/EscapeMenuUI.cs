using UnityEngine;
using TMPro;

public class EscapeMenuUI : UI {
    [SerializeField] UnityEngine.Audio.AudioMixerGroup masterMixer;
    [SerializeField] UnityEngine.Audio.AudioMixerGroup effectMixer;
    [SerializeField] UnityEngine.Audio.AudioMixerGroup musicMixer;

    [SerializeField] UnityEngine.UI.Slider masterSlider;
    [SerializeField] UnityEngine.UI.Slider effectSlider;
    [SerializeField] UnityEngine.UI.Slider musicSlider;
    public override bool isActive => canvas.activeInHierarchy;

    public void Start() {
        float value;
        if(float.TryParse(PlayerPrefs.GetString("Master Volume"), out value)) {
            masterMixer.audioMixer.SetFloat("Master", value);
            masterSlider.value = value;
        }
        if(float.TryParse(PlayerPrefs.GetString("Effect Volume"), out value)) {
            masterMixer.audioMixer.SetFloat("Effect", value);
            effectSlider.value = value;
        }
        if(float.TryParse(PlayerPrefs.GetString("Music Volume"), out value)) {
            masterMixer.audioMixer.SetFloat("Music", value);
            musicSlider.value = value;
        }
    }

    public override void OnActive() {
        canvas.SetActive(true);
        activeEvent.Invoke();
        GameManager.instance.StopGame();
    }
    public override void OnInactive() {
        GameManager.instance.StartGame();
        inactiveEvent.Invoke();
        canvas.SetActive(false);
    }
    public void SetMasterVolume() {
        float v = masterSlider.value<=-20 ? -80 : masterSlider.value;
        masterMixer.audioMixer.SetFloat("Master", v);
        PlayerPrefs.SetString("Master Volume", v+"");
    }
    public void SetEffectVolume() {
        float v = effectSlider.value<=-20 ? -80 : effectSlider.value;
        effectMixer.audioMixer.SetFloat("Effect", v);
        PlayerPrefs.SetString("Effect Volume", v+"");
    }
    public void SetMusicVolume() {
        float v = musicSlider.value<=-20 ? -80 : musicSlider.value;
        musicMixer.audioMixer.SetFloat("Music", v);
        PlayerPrefs.SetString("Music Volume", v+"");
    }
}