using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour
{

[SerializeField]    private Dropdown factionColorDropdown;
[SerializeField] private Dropdown graphicsDropdown;

[SerializeField] Slider masterVol, musicVol, sfxVol;

    // Start is called before the first frame update
    void Start()
    {
   
        // audio
        masterVol.value = MatchManager.instance.masterVolume;
        musicVol.value = MusicManager.instance.musicVolume;
        sfxVol.value = MatchManager.instance.sfxVolume;

    }

    // Update is called once per frame
    void Update()
    {
        // Update UI elements based on current settings (e.g., load saved settings)
        masterVol.onValueChanged.AddListener(SetMasterVolume);

    }

    // Callback for audio settings selection
    void SetMasterVolume(float value)
    {
        // Get the selected audio volume level
        MatchManager.instance.masterVolume = value;


    }
}