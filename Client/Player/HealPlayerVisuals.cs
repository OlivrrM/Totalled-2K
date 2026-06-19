using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPlayerVisuals : MonoBehaviour
{
    public PostFxVolumeManager healPostFxManager;
    float currentPostFxTime;
    bool disabledPostFx;
    float currentPostFxDisableSmoothness = 99f;
    private void Start()
    {
        currentPostFxDisableSmoothness = 99f;
        healPostFxManager.Disable(99f);
    }
    public void Play(HealPlayer healPlayer)
    {
        healPostFxManager.Enable(30f, healPlayer.postFxAmount);
        currentPostFxTime = healPlayer.postFxTime;
        currentPostFxDisableSmoothness = healPlayer.postFxDisableSmoothness;
        disabledPostFx = false;
    }
    private void Update()
    {
        currentPostFxTime -= Time.deltaTime;
        if (currentPostFxTime < 0f&&!disabledPostFx)
        {
            healPostFxManager.Disable(currentPostFxDisableSmoothness);
            disabledPostFx = true;
        }
    }
}
