using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;


public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown factionColorDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;


    public AudioMixer audioMixer;

    [SerializeField] private Slider masterVol, musicVol, sfxVol;

    // Start is called before the first frame update
    private void Start()
    {

        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        resolutions = Screen.resolutions;
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " +
                            resolutions[i].height;
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width
                && resolutions[i].height == Screen.currentResolution.height)
                currentResolutionIndex = i;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.RefreshShownValue();

        // audio
        // masterVol.value = MatchManager.instance.masterVolume;
        // musicVol.value = MusicManager.instance.musicVolume;
        // sfxVol.value = MatchManager.instance.sfxVolume;
    }

    // Update is called once per frame
    private void Update()
    {
        // Update UI elements based on current settings (e.g., load saved settings)
        masterVol.onValueChanged.AddListener(SetMasterVolume);
    }

    // Callback for audio settings selection
    private void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", value);
        // Get the selected audio volume level
        MatchManager.instance.masterVolume = value;
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width,
            resolution.height, Screen.fullScreen);
    }

}