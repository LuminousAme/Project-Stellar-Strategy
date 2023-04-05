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
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    private Resolution[] resolutions;

    [Space]
    [Header("Faction Colors")]
    [SerializeField] private TMP_Dropdown factionColorDropdown;
    [SerializeField] private TMP_Dropdown ai1factionColorDropdown;
    [SerializeField] private TMP_Dropdown ai2factionColorDropdown;
    public Image playerColorPreview;
    public Image ai1ColorPreview;
    public Image ai2ColorPreview;

    SceneTransition transition;

    // Dictionary of color names and their RGB values
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
        {"Light Pastel Green", new  Color(0.698039216f, 0.984313725f, 0.647058824f)},
        {"Green Teal", new Color(0.0470588235f, 0.709803922f, 0.466666667f)},
        {"Strong Pink", new Color(1f, 0.0274509804f, 0.537254902f)},
        {"Bland", new Color(0.68627451f, 0.658823529f, 0.545098039f)},
        {"Deep Aqua", new Color(0.031372549f, 0.470588235f, 0.498039216f)},
        {"Lavender Pink", new Color(0.866666667f, 0.521568627f, 0.843137255f)},
        {"Light Moss Green", new Color(0.650980392f, 0.784313725f, 0.458823529f)},
        {"Light Seafoam Green", new Color(0.654901961f, 1f, 0.709803922f)},
        {"Olive Yellow", new Color(0.760784314f, 0.717647059f, 0.0352941176f)}
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

        List<string> colorNames = new List<string>(colorDict.Keys); //list of names

        //if color is not in dictionary, then add it with the name default
        if (!colorDict.ContainsValue(player.passiveColor))
        {
            // add player.passiveColor to colorDict with name "Default" and add it to the top of the dictionary
            colorDict = colorDict.OrderByDescending(x => x.Key == "Player Default").ToDictionary(x => x.Key, x => x.Value);
            colorDict.Add("Player Default", player.passiveColor);
            colorNames.Insert(0, "Player Default");
        }

        //if color is not in dictionary, then add it with the name default
        if (!colorDict.ContainsValue(ai1.passiveColor))
        {
            // add player.passiveColor to colorDict with name "Default" and add it to the top of the dictionary
            colorDict = colorDict.OrderByDescending(x => x.Key == "AI 1 Default").ToDictionary(x => x.Key, x => x.Value);
            colorDict.Add("AI 1 Default", ai1.passiveColor);
            colorNames.Insert(0, "AI 1 Default");
        }

        //if color is not in dictionary, then add it with the name default
        if (!colorDict.ContainsValue(ai2.passiveColor))
        {
            // add player.passiveColor to colorDict with name "Default" and add it to the top of the dictionary
            colorDict = colorDict.OrderByDescending(x => x.Key == " AI 2 Default").ToDictionary(x => x.Key, x => x.Value);
            colorDict.Add("AI 2 Default", ai2.passiveColor);
            colorNames.Insert(0, "AI 2 Default");
        }

        factionColorDropdown.AddOptions(colorNames);
        ai1factionColorDropdown.AddOptions(colorNames);
        ai2factionColorDropdown.AddOptions(colorNames);
        
        // Check if player's passive color is in the dictionary
        if (colorDict.ContainsValue(player.passiveColor))
        { // Set the dropdown value to the corresponding color name
            factionColorDropdown.value = colorNames.IndexOf(colorDict.FirstOrDefault(x => x.Value == player.passiveColor).Key);
            playerColorPreview.color = player.passiveColor;
        }
    
        // Check if ai1's passive color is in the dictionary
        if (colorDict.ContainsValue(ai1.passiveColor))
        { // Set the dropdown value to the corresponding color name
            ai1factionColorDropdown.value = colorNames.IndexOf(colorDict.FirstOrDefault(x => x.Value == ai1.passiveColor).Key);
            ai1ColorPreview.color = ai1.passiveColor;
        }
        // Check if ai2's passive color is in the dictionary
        if (colorDict.ContainsValue(ai2.passiveColor))
        { // Set the dropdown value to the corresponding color name
            ai2factionColorDropdown.value = colorNames.IndexOf(colorDict.FirstOrDefault(x => x.Value == ai2.passiveColor).Key);
            ai2ColorPreview.color = ai2.passiveColor;

        }


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

        transition = GetComponent<SceneTransition>();
    }

    // Update is called once per frame
    private void Update()
    {
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
        playerColorPreview.color = selectedColor;
    }

    public void OnColorSelectedAI1(int index)
    {
        // Get selected color value from dictionary and set it on the Faction
        Color selectedColor = colorDict[ai1factionColorDropdown.options[index].text];
        //  MatchManager.instance.aiFactions[0].passiveColor = selectedColor;
        ai1.passiveColor = selectedColor;
        ai1ColorPreview.color = selectedColor;
    }

    public void OnColorSelectedAI2(int index)
    {
        // Get selected color value from dictionary and set it on the Faction
        Color selectedColor = colorDict[ai2factionColorDropdown.options[index].text];
        // MatchManager.instance.aiFactions[0].passiveColor = selectedColor;
        ai2.passiveColor = selectedColor;
        ai2ColorPreview.color = selectedColor;
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
        transition.beginTransition("SampleScene");
    }
}