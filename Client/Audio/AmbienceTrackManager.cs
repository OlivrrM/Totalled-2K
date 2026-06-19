using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceTrackManager : MonoBehaviour
{
    public Dictionary<string,AmbienceTrack> ambienceTracks = new Dictionary<string, AmbienceTrack>();
    AmbienceTrack currentTrack;

    float musicPlayingVolumeMultiplier;

    private void Awake()
    {
        Cache.ambienceTrackManager = this;
    }
    public void AddAmbienceTrack(string identifier, AmbienceTrack track)
    {
        ambienceTracks.Add(identifier, track);
    }
    public void RemoveAmbienceTrack(string identifier)
    {
        ambienceTracks.Remove(identifier);
    }
    public void SetCurrentTrack(string identifier)
    {
        if (ambienceTracks.ContainsKey(identifier)){
            currentTrack = ambienceTracks[identifier];
        }
        else { Debug.LogError($"Tried to set current ambience track to '{identifier}' however no track with such identifier is currently loaded"); }
    }
    public void SetTrackVolume(string identifier, float volume)
    {
        if (ambienceTracks.ContainsKey(identifier)) {
            ambienceTracks[identifier].volume = volume;
        }
        else { Debug.LogError($"Tried to set volumee of ambience track '{identifier}' to {volume}, however no track with such identifier is currently loaded"); }
    }
    private void Update()
    {
        if (currentTrack != null){
            foreach (KeyValuePair<string, AmbienceTrack> track in ambienceTracks){
                if (track.Value != currentTrack){
                    track.Value.audioSource.volume = Mathf.Lerp(track.Value.audioSource.volume, 0f, Time.deltaTime * 0.5f);
                }
            }
            currentTrack.audioSource.volume = Mathf.Lerp(currentTrack.audioSource.volume, currentTrack.volume*musicPlayingVolumeMultiplier, Time.deltaTime * 0.5f);
        }
        musicPlayingVolumeMultiplier = Mathf.Lerp(musicPlayingVolumeMultiplier, MusicTrackManager.musicTrackPlaying ? 0.1f : 1f, Time.deltaTime);
    }
}
