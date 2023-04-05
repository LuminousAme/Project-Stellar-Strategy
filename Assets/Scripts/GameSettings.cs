using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GameSettings : MonoBehaviour
{
    public static GameSettings instance;

    public float masterVolume, musicVolume, SFXVolume;
    public bool fullscreen;
    public int graphicsQuality, resolutionIndex;
    public string playerfactionColor, ai1factionColor, ai2factionColor;

    [SerializeField] AudioMixer mixer;
    Resolution[] resolutions;

    public string LastScene = "";

    // Dictionary of color names and their RGB values
    public Dictionary<string, Color> colorDict = new Dictionary<string, Color>
    {
        {"Green", Color.green},
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

    public Faction player, ai1, ai2;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            LastScene = "MainMenu";
            ReadValuesFromFile();
        }
    }

    void ReadValuesFromFile()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0);
        mixer.SetFloat("MasterVolume", masterVolume);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0);
        mixer.SetFloat("MusicVolume", musicVolume);
        SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 0);
        mixer.SetFloat("SFXVolume", SFXVolume);

        int fullscreenint = PlayerPrefs.GetInt("Fullscreen", 0);
        fullscreen = fullscreenint == 1;
        Screen.fullScreen = fullscreen;

        graphicsQuality = PlayerPrefs.GetInt("GraphicsQuality", QualitySettings.GetQualityLevel());
        QualitySettings.SetQualityLevel(graphicsQuality);

        resolutions = Screen.resolutions;
        resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", resolutions.Length - 1);
        if (resolutionIndex >= resolutions.Length)
        {
            resolutionIndex = resolutions.Length - 1;
            SaveValuesToFile();
        }
        Resolution newResolution = resolutions[resolutionIndex];
        Screen.SetResolution(newResolution.width, newResolution.height, Screen.fullScreen);

        playerfactionColor = PlayerPrefs.GetString("PlayerFactionColor", "Green");
        if(colorDict.ContainsKey(playerfactionColor))
        {
            player.selectedColor = colorDict[playerfactionColor];
        }
        ai1factionColor = PlayerPrefs.GetString("AI1FactionColor", "Blue");
        if (colorDict.ContainsKey(ai1factionColor))
        {
            ai1.passiveColor = colorDict[ai1factionColor];
            ai1.selectedColor = colorDict[ai1factionColor];
        }
        ai2factionColor = PlayerPrefs.GetString("AI2FactionColor", "Yellow");
        if (colorDict.ContainsKey(ai2factionColor))
        {
            ai2.passiveColor = colorDict[ai2factionColor];
            ai2.selectedColor = colorDict[ai2factionColor];
        }
    }

    public void SaveValuesToFile()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.GetFloat("SFXVolume", SFXVolume);

        int fullscreenInt = (fullscreen) ? 1 : 0;
        PlayerPrefs.SetInt("Fullscreen", fullscreenInt);

        PlayerPrefs.SetInt("GraphicsQuality", graphicsQuality);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);

        PlayerPrefs.SetString("PlayerFactionColor", playerfactionColor);
        PlayerPrefs.SetString("AI1FactionColor", ai1factionColor);
        PlayerPrefs.SetString("AI2FactionColor", ai2factionColor);
    }

    private void OnApplicationQuit()
    {
        SaveValuesToFile();
    }
}
