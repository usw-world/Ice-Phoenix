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
        masterMixer.audioMixer.SetFloat("Master", masterSlider.value<=-20 ? -80 : masterSlider.value);
    }
    public void SetEffectVolume() {
        effectMixer.audioMixer.SetFloat("Effect", effectSlider.value<=-20 ? -80 : effectSlider.value);
    }
    public void SetMusicVolume() {
        musicMixer.audioMixer.SetFloat("Music", musicSlider.value<=-20 ? -80 : musicSlider.value);
    }
}