using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusicTrack : MonoBehaviour
{
    public AudioSource audioSource;
    AudioClip clip;
    float clipLength;
    float currentClipTime;
    private void Start()
    {
        clip = audioSource.clip;
        clipLength = clip.length;
    }
    public void Play()
    {
        audioSource.Play();
        MusicTrackManager.musicTrackPlaying = true;
        currentClipTime = 0f;
    }
    private void Update()
    {
        if (audioSource.isPlaying){
            currentClipTime += Time.deltaTime;
            if (currentClipTime> clipLength){
                MusicTrackManager.musicTrackPlaying = false;
            }
        }
    }
}
