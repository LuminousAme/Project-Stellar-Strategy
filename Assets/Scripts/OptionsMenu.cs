using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

//https://www.red-gate.com/simple-talk/development/dotnet-development/how-to-create-a-settings-menu-in-unity/ 
public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown factionColorDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    Resolution[] resolutions;


    public AudioMixer audioMixer;

    [SerializeField] private Slider masterVol, musicVol, sfxVol;
    
    // Dictionary of color names and RGB values
    private Dictionary<string, Color> colorDict = new Dictionary<string, Color>
    {
        {"Red", Color.red},
        {"Green", Color.green},
        {"Blue", Color.blue},
        {"Yellow", Color.yellow},
        {"Cyan", Color.cyan},
        {"Magenta", Color.magenta},
        {"White", Color.white},
        {"Black", Color.black},
    };

    // Start is called before the first frame update
    private void Start()
    {
        factionColorDropdown.ClearOptions();

        List<string> colorNames = new List<string>(colorDict.Keys);
       
        factionColorDropdown.AddOptions(colorNames);
        factionColorDropdown.RefreshShownValue();

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

        qualityDropdown.RefreshShownValue();

        // audio
         masterVol.value = MatchManager.instance.masterVolume;
        musicVol.value = MusicManager.instance.musicVolume;
         sfxVol.value = MatchManager.instance.sfxVolume;
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



    public void OnColorSelected(int index)
    {
        // Get selected color value from dictionary and set it on the Faction
        Color selectedColor = colorDict[factionColorDropdown.options[index].text];
        MatchManager.instance.playerFaction.passiveColor = selectedColor;
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

    public void SetQuality(int qIndex)
    {
        QualitySettings.SetQualityLevel(qIndex);
    }

    public void SetTextureQuality(int textureIndex)
    {
        QualitySettings.masterTextureLimit = textureIndex;
        qualityDropdown.value = 6;
    }

}