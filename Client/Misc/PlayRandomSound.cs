using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRandomSound : MonoBehaviour
{
    public Transform poolParent;
    PlaySound[] playSounds;

    public bool playOnAwake;
    public bool destroyAfterPlayed;
    float lifetime;
    float currentLifetime;
    T[] GetComponentsInChildren<T>() where T : Component
    {
        return gameObject.GetComponentsInChildren<T>();
    }
    private void Awake()
    {
        playSounds = GetComponentsInChildren<PlaySound>();
        if (playOnAwake) { Play(); }
    }
    public void Play()
    {
        int chosenSound = Random.Range(0, playSounds.Length);
        playSounds[chosenSound].Play();
        lifetime = playSounds[chosenSound].SFX.clip.length;
    }
    public void Play(float pitchA, float pitchB)
    {
        int chosenSound = Random.Range(0, playSounds.Length);
        playSounds[chosenSound].pitchA = pitchA;
        playSounds[chosenSound].pitchB = pitchB;
        playSounds[chosenSound].Play();
    }
    public void Play(float pitchA, float pitchB, float volume)
    {
        int chosenSound = Random.Range(0, playSounds.Length);
        playSounds[chosenSound].pitchA = pitchA;
        playSounds[chosenSound].pitchB = pitchB;
        playSounds[chosenSound].SFX.volume = volume;
        playSounds[chosenSound].Play();
    }
    void Update()
    {
        if (destroyAfterPlayed&& lifetime!=0)
        {
            currentLifetime += Time.deltaTime;
            if (currentLifetime > lifetime) Destroy(gameObject);
        }
    }
}
