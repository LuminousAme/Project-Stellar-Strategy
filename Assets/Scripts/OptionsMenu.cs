using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//https://www.red-gate.com/simple-talk/development/dotnet-development/how-to-create-a-settings-menu-in-unity/
public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown factionColorDropdown, ai1factionColorDropdown, ai2factionColorDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    private Resolution[] resolutions;

 
    // Dictionary of color names and RGB values
    private Dictionary<string, Color> colorDict = new Dictionary<string, Color>
    {
        {"Red", Color.red},
        {"Blue", Color.blue},
        {"Yellow", Color.yellow},
        {"Cyan", Color.cyan},
        {"Magenta", Color.magenta},
        {"White", Color.white},
        {"Black", Color.black},
        {"Grey", Color.gray},
    };

    [SerializeField] private Faction player, ai1, ai2;

    [Space]
    [Header("Audio")]
    public AudioMixer audioMixer;

    [SerializeField] private Slider masterVol, musicVol, sfxVol;

    // Start is called before the first frame update
    private void Start()
    {
        factionColorDropdown.ClearOptions();
        ai1factionColorDropdown.ClearOptions();
        ai2factionColorDropdown.ClearOptions();

        List<string> colorNames = new List<string>(colorDict.Keys);

        factionColorDropdown.AddOptions(colorNames);
        ai1factionColorDropdown.AddOptions(colorNames);
        ai2factionColorDropdown.AddOptions(colorNames);
        factionColorDropdown.RefreshShownValue();
        ai1factionColorDropdown.RefreshShownValue();
        ai2factionColorDropdown.RefreshShownValue();

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
        // MatchManager.instance.playerFaction.passiveColor = selectedColor;
        player.passiveColor = selectedColor;
    }

    public void OnColorSelectedAI1(int index)
    {
        // Get selected color value from dictionary and set it on the Faction
        Color selectedColor = colorDict[ai1factionColorDropdown.options[index].text];
        // MatchManager.instance.playerFaction.passiveColor = selectedColor;
        ai1.passiveColor = selectedColor;
    }

    public void OnColorSelectedAI2(int index)
    {
        // Get selected color value from dictionary and set it on the Faction
        Color selectedColor = colorDict[ai2factionColorDropdown.options[index].text];
        // MatchManager.instance.playerFaction.passiveColor = selectedColor;
        ai2.passiveColor = selectedColor;
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
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

    //go to game rn just for dev purposes
    public void Back()
    {
        SceneManager.LoadScene("SampleScene");
    }
}