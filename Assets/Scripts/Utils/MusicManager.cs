using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public Song[] songs;
    private List<Song> songInstances = new List<Song>();
    private Song currentSong = null;
    private Song oldSong = null;

    public float musicVolume; //doesnt actually do anything rn

    static public MusicManager instance = null;

    private void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        CreateAudioSources();
        currentSong = null;
        oldSong = null;

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void CreateAudioSources()
    {
        for (int i = 0; i < songs.Length; i++)
        {
            songInstances.Add(ScriptableObject.CreateInstance<Song>());
            songInstances[i].name = songs[i].name;
            songInstances[i].tracks = songs[i].tracks;
            songInstances[i].volume = songs[i].volume;
            songInstances[i].targetMixerGroup = songs[i].targetMixerGroup;
            songInstances[i].intesnityLevel = songs[i].intesnityLevel;

            for (int j = 0; j < songInstances[i].tracks.Length; j++)
            {
                GameObject newObj = new GameObject("Music - Song: " + songInstances[i].name + " Track: " + songInstances[i].tracks[j].trackName, typeof(AudioSource));
                newObj.transform.SetParent(transform);
                AudioSource source = newObj.GetComponent<AudioSource>();
                source.playOnAwake = false;
                source.clip = songInstances[i].tracks[j].file;
                source.volume = 0.0f;
                songInstances[i].tracks[j].volume = 0.0f;
                source.outputAudioMixerGroup = songInstances[i].targetMixerGroup;
                source.loop = true;
                source.Stop();
                songInstances[i].tracks[j].source = source;
            }
        }
    }

    private void Update()
    {
        UpdateFadeIn();
        UpdateFadeOut();
        UpdateTrackVolume(oldSong);
        UpdateTrackVolume(currentSong);
    }

    private void UpdateFadeIn()
    {
        if (currentSong == null) return;

        for (int i = 0; i < currentSong.tracks.Length; i++)
        {
            Track track = currentSong.tracks[i];
            if (track.intensityLevel <= currentSong.intesnityLevel)
            {
                if (track.fadeTime <= 0.0f)
                {
                    track.volume = currentSong.volume;
                    continue;
                }

                float t = Mathf.Clamp01(track.fadeElapsed / track.fadeTime);
                track.volume = Mathf.Lerp(track.cachedVolume, currentSong.volume, t);
                track.fadeElapsed += Time.deltaTime;
            }
            else if (track.volume > 0.0f)
            {
                if (track.fadeTime <= 0.0f)
                {
                    track.volume = 0.0f;
                    continue;
                }

                float t = Mathf.Clamp01(track.fadeElapsed / track.fadeTime);
                track.volume = Mathf.Lerp(track.cachedVolume, 0.0f, t);
                track.fadeElapsed += Time.deltaTime;
            }
        }
    }

    private void UpdateFadeOut()
    {
        if (oldSong == null) return;

        for (int i = 0; i < oldSong.tracks.Length; i++)
        {
            Track track = oldSong.tracks[i];
            if (track.intensityLevel <= currentSong.intesnityLevel)
            {

                if (track.fadeTime <= 0.0f)
                {
                    track.volume = 0.0f;
                    continue;
                }

                float t = Mathf.Clamp01(track.fadeElapsed / track.fadeTime);
                track.volume = Mathf.Lerp(track.cachedVolume, 0.0f, t);
                track.fadeElapsed += Time.deltaTime;
            }
            else
            {
                track.volume = 0.0f;
            }
        }
    }

    private void UpdateTrackVolume(Song song)
    {
        if (song == null) return;

        for (int i = 0; i < song.tracks.Length; i++)
        {
            Track track = song.tracks[i];
            AudioSource source = track.source;
            source.volume = track.volume;
        }
    }

    public int CurrentIndex()
    {
        if (currentSong == null) return -1;

        return songInstances.IndexOf(currentSong);
    }

    public int GetCurrentIntensity()
    {
        if (currentSong == null) return -1;

        return currentSong.intesnityLevel;
    }

    public void SetCurrentIntensity(int intensity)
    {
        if (currentSong == null) return;

        currentSong.intesnityLevel = intensity;
    }


    public void PlayAllTracksImmediate(int index)
    {
        if (index < 0 || index > songInstances.Count - 1) return;

        if (currentSong != null)
        {
            SetFadeTimes(currentSong, 0.0f);
            currentSong.intesnityLevel = -1;
        }

        currentSong = songInstances[index];
        oldSong = null;

        for (int i = 0; i < currentSong.tracks.Length; i++)
        {
            Track track = currentSong.tracks[i];
            track.volume = currentSong.volume;
            track.cachedVolume = track.volume;
            track.fadeTime = 0.0f;

            AudioSource source = track.source;
            source.volume = track.volume;
            source.Play();
        }
    }

    public void FadeTracksIn(int index, int intensityLevel, float fadeInTime)
    {
        if (index < 0 || index > songInstances.Count - 1) return;

        oldSong = currentSong;
        currentSong = songInstances[index];

        if (oldSong != null)
        {
            SetFadeTimes(oldSong, fadeInTime);
            for (int i = 0; i < oldSong.tracks.Length; i++)
            {
                Track track = oldSong.tracks[i];
                track.cachedVolume = track.volume;
            }
        }
        SetFadeTimes(currentSong, fadeInTime);
        currentSong.intesnityLevel = intensityLevel;

        for (int i = 0; i < currentSong.tracks.Length; i++)
        {
            Track track = currentSong.tracks[i];
            track.cachedVolume = track.volume;
            track.volume = 0.0f;

            AudioSource source = track.source;
            source.volume = track.volume;
            source.Play();
        }
    }

    public void ChangeIntensity(int intensnityLevel)
    {
        currentSong.intesnityLevel = intensnityLevel;

        for (int i = 0; i < currentSong.tracks.Length; i++)
        {
            Track track = currentSong.tracks[i];
            if (track.intensityLevel < intensnityLevel)
            {
                track.fadeElapsed = 0.0f;
            }
        }
    }

    private void SetFadeTimes(Song song, float fadeTime)
    {
        for (int i = 0; i < song.tracks.Length; i++)
        {
            Track track = song.tracks[i];
            track.fadeTime = fadeTime;
            track.fadeElapsed = 0.0f;
        }
    }
}
