using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public Song[] songs;
    private Song currentSong = null;
    private Song oldSong = null;

    public MusicManager instance = null;

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
            for (int j = 0; j < songs[i].tracks.Length; j++)
            {
                GameObject newObj = new GameObject("Music - Song: " + songs[i].name + " Track: " + songs[i].tracks[j].trackName, typeof(AudioSource));
                newObj.transform.SetParent(transform);
                AudioSource source = newObj.GetComponent<AudioSource>();
                source.playOnAwake = false;
                source.clip = songs[i].tracks[j].file;
                source.volume = 0.0f;
                songs[i].tracks[j].volume = 0.0f;
                source.outputAudioMixerGroup = songs[i].targetMixerGroup;
                source.loop = true;
                source.Stop();
                songs[i].tracks[j].source = source;
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
            if (track.intensityLevel >= currentSong.intesnityLevel)
            {
                if (track.fadeTime <= 0.0f)
                {
                    track.volume = currentSong.volume;
                    continue;
                }

                float t = Mathf.Clamp01(track.fadeElapsed / track.fadeTime);
                track.volume = Mathf.Lerp(0.0f, currentSong.volume, t);
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
                track.volume = Mathf.Lerp(currentSong.volume, 0.0f, t);
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
            if (track.fadeTime <= 0.0f)
            {
                track.volume = 0.0f;
                continue;
            }

            float t = Mathf.Clamp01(track.fadeElapsed / track.fadeTime);
            track.volume = Mathf.Lerp(currentSong.volume, 0.0f, t);
            track.fadeElapsed += Time.deltaTime;
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


    public void PlayAllTracksImmediate(int index)
    {
        if (index < 0 || index > songs.Length - 1) return;

        if (currentSong != null)
        {
            SetFadeTimes(currentSong, 0.0f);
            currentSong.intesnityLevel = -1;
        }

        currentSong = songs[index];
        oldSong = null;

        for (int i = 0; i < currentSong.tracks.Length; i++)
        {
            Track track = currentSong.tracks[i];
            track.volume = currentSong.volume;
            track.fadeTime = 0.0f;

            AudioSource source = track.source;
            source.volume = track.volume;
            source.Play();
        }
    }

    public void FadeTracksIn(int index, int intensityLevel, float fadeInTime)
    {
        if (index < 0 || index > songs.Length - 1) return;

        oldSong = currentSong;
        currentSong = songs[index];

        if (oldSong != null)
        {
            SetFadeTimes(oldSong, 0.0f);
            oldSong.intesnityLevel = -1;
        }
        SetFadeTimes(currentSong, fadeInTime);
        currentSong.intesnityLevel = intensityLevel;

        for (int i = 0; i < currentSong.tracks.Length; i++)
        {
            Track track = currentSong.tracks[i];
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
