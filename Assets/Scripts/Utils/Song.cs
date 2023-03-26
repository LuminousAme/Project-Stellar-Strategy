using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "Song", menuName = "CelestialConquest/SongObj")]
public class Song : ScriptableObject
{
    public string songName;
    public Track[] tracks;
    [Range(0f, 1f)]
    public float volume = 0.5f;
    public AudioMixerGroup targetMixerGroup;
    public int intesnityLevel = int.MaxValue;
}

[System.Serializable]
public class Track
{
    public string trackName;
    public AudioClip file;
    public int intensityLevel = 0;
    [HideInInspector] public float volume = 0.0f;
    [HideInInspector] public float cachedVolume = 0.0f;
    [HideInInspector] public float fadeTime = 0.0f;
    [HideInInspector] public float fadeElapsed = 0.0f;
    [HideInInspector] public AudioSource source;
}