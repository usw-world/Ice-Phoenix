using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionOption : MonoBehaviour
{
    FullScreenMode screenMode;
    public Dropdown dropdown;
    public Toggle fullScreenCheck;
    int resolutionNum;
    List<Resolution> resolutions = new List<Resolution>();

    void Start()
    {
        Resolution();
    }
    void Resolution()
    {
        for(int i = 0; i < Screen.resolutions.Length; i++)
        {
            if(Screen.resolutions[i].refreshRate == 60)
                resolutions.Add(Screen.resolutions[i]);
        }

        dropdown.options.Clear();

        int optionNum = 0;
        foreach(Resolution item in resolutions)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = item.width + " X " + item.height + " ";
            dropdown.options.Add(option);

            if(item.width == Screen.width && item.height == Screen.height)
                dropdown.value = optionNum;
            optionNum++;                
        }

        dropdown.RefreshShownValue();
        fullScreenCheck.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow) ? true : false;
    }

    public void DropboxOptionChange(int x)
    {
        resolutionNum = x;
    }

    public void FullScreenCheck(bool isFull)
    {
        screenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    public void OkBtnClick()
    {
        Screen.SetResolution(resolutions[resolutionNum].width, resolutions[resolutionNum].height, screenMode);
    }
}