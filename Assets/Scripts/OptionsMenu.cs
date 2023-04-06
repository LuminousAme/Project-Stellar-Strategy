using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//https://www.red-gate.com/simple-talk/development/dotnet-development/how-to-create-a-settings-menu-in-unity/
public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject EventSystem;
    [SerializeField] private Image background;
    //[SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    //private Resolution[] resolutions;

    [Space]
    [Header("Faction Colors")]
    [SerializeField] private TMP_Dropdown factionColorDropdown;
    [SerializeField] private TMP_Dropdown ai1factionColorDropdown;
    [SerializeField] private TMP_Dropdown ai2factionColorDropdown;
    public Image playerColorPreview;
    public Image ai1ColorPreview;
    public Image ai2ColorPreview;

    [SerializeField] private SceneTransition transition;

    [Space]
    [Header("Audio")]
    public AudioMixer audioMixer;

    [SerializeField] private Slider masterVol, musicVol, sfxVol;

    // Start is called before the first frame update
    private void Start()
    {
        if (GameSettings.instance.LastScene != "MainMenu")
        {
            transition.gameObject.SetActive(false);
            cam.gameObject.SetActive(false);
            EventSystem.SetActive(false);
            Image image = transition.GetImage();
            Color fadecol = image.color;
            fadecol.a = 0.0f;
            image.color = fadecol;
            image.raycastTarget = false;
            image.maskable = false;

            Color col = background.color;
            col.a = 1.0f;
            background.color = col;
        }
        else
        {
            Color col = background.color;
            col.a = 0.0f;
            background.color = col;
        }

        factionColorDropdown.ClearOptions();
        ai1factionColorDropdown.ClearOptions();
        ai2factionColorDropdown.ClearOptions();

        List<string> colorNames = new List<string>(GameSettings.instance.colorDict.Keys); //list of names

        //get the color from the settings
        if(GameSettings.instance.colorDict.ContainsKey(GameSettings.instance.playerfactionColor))
        {
            GameSettings.instance.player.selectedColor = GameSettings.instance.colorDict[GameSettings.instance.playerfactionColor];
        }
        //if color is not in dictionary, then add it with the name default
        else if (!GameSettings.instance.colorDict.ContainsValue(GameSettings.instance.player.selectedColor))
        {
            // add player.passiveColor to colorDict with name "Default" and add it to the top of the dictionary
            GameSettings.instance.colorDict = GameSettings.instance.colorDict.OrderByDescending(x => x.Key == "Player Default").ToDictionary(x => x.Key, x => x.Value);
            GameSettings.instance.colorDict.Add("Player Default", GameSettings.instance.player.selectedColor);
            colorNames.Insert(0, "Player Default");
            GameSettings.instance.playerfactionColor = "Player Default";
        }

        //get the color from the settings
        if (GameSettings.instance.colorDict.ContainsKey(GameSettings.instance.ai1factionColor))
        {
            GameSettings.instance.ai1.passiveColor = GameSettings.instance.colorDict[GameSettings.instance.ai1factionColor];
            GameSettings.instance.ai1.selectedColor = GameSettings.instance.colorDict[GameSettings.instance.ai1factionColor];
        }
        //if color is not in dictionary, then add it with the name default
        else if (!GameSettings.instance.colorDict.ContainsValue(GameSettings.instance.ai1.passiveColor))
        {
            // add player.selectedColor to colorDict with name "Default" and add it to the top of the dictionary
            GameSettings.instance.colorDict = GameSettings.instance.colorDict.OrderByDescending(x => x.Key == "AI 1 Default").ToDictionary(x => x.Key, x => x.Value);
            GameSettings.instance.colorDict.Add("AI 1 Default", GameSettings.instance.ai1.passiveColor);
            colorNames.Insert(0, "AI 1 Default");
            GameSettings.instance.ai1factionColor = "AI 1 Default";
        }


        //get the color from the settings
        if (GameSettings.instance.colorDict.ContainsKey(GameSettings.instance.ai2factionColor))
        {
            GameSettings.instance.ai2.passiveColor = GameSettings.instance.colorDict[GameSettings.instance.ai2factionColor];
            GameSettings.instance.ai2.selectedColor = GameSettings.instance.colorDict[GameSettings.instance.ai2factionColor];
        }
        //if color is not in dictionary, then add it with the name default
        else if (!GameSettings.instance.colorDict.ContainsValue(GameSettings.instance.ai2.passiveColor))
        {
            // add player.selectedColor to colorDict with name "Default" and add it to the top of the dictionary
            GameSettings.instance.colorDict = GameSettings.instance.colorDict.OrderByDescending(x => x.Key == " AI 2 Default").ToDictionary(x => x.Key, x => x.Value);
            GameSettings.instance.colorDict.Add("AI 2 Default", GameSettings.instance.ai2.passiveColor);
            colorNames.Insert(0, "AI 2 Default");
            GameSettings.instance.ai2factionColor = "AI 2 Default";
        }



        factionColorDropdown.AddOptions(colorNames);
        ai1factionColorDropdown.AddOptions(colorNames);
        ai2factionColorDropdown.AddOptions(colorNames);
        
        // Check if player's passive color is in the dictionary
        if (GameSettings.instance.colorDict.ContainsValue(GameSettings.instance.player.selectedColor))
        { // Set the dropdown value to the corresponding color name
            factionColorDropdown.value = colorNames.IndexOf(GameSettings.instance.colorDict.FirstOrDefault(x => x.Value == GameSettings.instance.player.selectedColor).Key);
            playerColorPreview.color = GameSettings.instance.player.selectedColor;
        }
    
        // Check if ai1's passive color is in the dictionary
        if (GameSettings.instance.colorDict.ContainsValue(GameSettings.instance.ai1.passiveColor))
        { // Set the dropdown value to the corresponding color name
            ai1factionColorDropdown.value = colorNames.IndexOf(GameSettings.instance.colorDict.FirstOrDefault(x => x.Value == GameSettings.instance.ai1.passiveColor).Key);
            ai1ColorPreview.color = GameSettings.instance.ai1.passiveColor;
        }
        // Check if ai2's passive color is in the dictionary
        if (GameSettings.instance.colorDict.ContainsValue(GameSettings.instance.ai2.passiveColor))
        { // Set the dropdown value to the corresponding color name
            ai2factionColorDropdown.value = colorNames.IndexOf(GameSettings.instance.colorDict.FirstOrDefault(x => x.Value == GameSettings.instance.ai2.passiveColor).Key);
            ai2ColorPreview.color = GameSettings.instance.ai2.passiveColor;
        }


        factionColorDropdown.RefreshShownValue();
        ai1factionColorDropdown.RefreshShownValue();
        ai2factionColorDropdown.RefreshShownValue();

		/*
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        resolutions = Screen.resolutions;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " +
                            resolutions[i].height;
							
            options.Add(option);
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = GameSettings.instance.resolutionIndex;
        resolutionDropdown.RefreshShownValue();
*/
        qualityDropdown.ClearOptions();
        List<string> qualityOptions = new List<string>();
        int currentQualityIndex = QualitySettings.GetQualityLevel();
        for(int i = 0; i < QualitySettings.names.Length; i++)
        {
            qualityOptions.Add(QualitySettings.names[i]);
        }
        qualityDropdown.AddOptions(qualityOptions);
        qualityDropdown.value = currentQualityIndex;
        qualityDropdown.RefreshShownValue();

        // audio
        masterVol.value = GameSettings.instance.masterVolume;
        musicVol.value = GameSettings.instance.musicVolume;
        sfxVol.value = GameSettings.instance.SFXVolume;
    }

    // Callback for audio settings selection
    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", value);
        GameSettings.instance.masterVolume = value;
    }

    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVolume", value);
        GameSettings.instance.musicVolume = value;
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("SFXVolume", value);
        GameSettings.instance.SFXVolume = value;
    }

    public void OnColorSelected(int index)
    {
        // Get selected color value from dictionary and set it on the Faction
        Color selectedColor = GameSettings.instance.colorDict[factionColorDropdown.options[index].text];
        // MatchManager.instance.playerFaction.passiveColor = selectedColor;
        GameSettings.instance.player.selectedColor = selectedColor;
        playerColorPreview.color = selectedColor;
        GameSettings.instance.playerfactionColor = factionColorDropdown.options[index].text;
    }

    public void OnColorSelectedAI1(int index)
    {
        // Get selected color value from dictionary and set it on the Faction
        Color selectedColor = GameSettings.instance.colorDict[ai1factionColorDropdown.options[index].text];
        //  MatchManager.instance.aiFactions[0].passiveColor = selectedColor;
        GameSettings.instance.ai1.passiveColor = selectedColor;
        GameSettings.instance.ai1.selectedColor = selectedColor;
        ai1ColorPreview.color = selectedColor;
        GameSettings.instance.ai1factionColor = ai1factionColorDropdown.options[index].text;
    }

    public void OnColorSelectedAI2(int index)
    {
        // Get selected color value from dictionary and set it on the Faction
        Color selectedColor = GameSettings.instance.colorDict[ai2factionColorDropdown.options[index].text];
        // MatchManager.instance.aiFactions[0].passiveColor = selectedColor;
        GameSettings.instance.ai2.passiveColor = selectedColor;
        GameSettings.instance.ai2.selectedColor = selectedColor;
        ai2ColorPreview.color = selectedColor;
        GameSettings.instance.ai2factionColor = ai2factionColorDropdown.options[index].text;
    }

    public void SetFullscreen()
    {
        //bool fullscreen = !GameSettings.instance.fullscreen;
        //GameSettings.instance.fullscreen = fullscreen;
		//Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        Screen.fullScreen = !Screen.fullScreen;
    }

	/*
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        GameSettings.instance.resolutionIndex = resolutionIndex;
    }
	*/

    public void SetQuality(int qIndex)
    {
        QualitySettings.SetQualityLevel(qIndex);
        GameSettings.instance.graphicsQuality = qIndex;
    }

    public void Back()
    {
        if (GameSettings.instance.LastScene == "MainMenu")
            transition.beginTransition("MainMenu");

        else
            SceneManager.UnloadSceneAsync("Options");
    }
}