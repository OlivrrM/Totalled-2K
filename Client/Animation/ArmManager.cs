using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmManager : MonoBehaviour
{
    public bool hidden;
    float currentShowHideSpeed;
    public Transform arms;
    public SkinnedMeshRenderer[] armRenderers;
    private void Start()
    {
        Cache.armManager = this;
    }
    private void Update()
    {
        if (hidden) {
            arms.localPosition = Vector3.Lerp(arms.localPosition, new Vector3(0, 0, -1f), Time.deltaTime * currentShowHideSpeed);
            if (arms.localPosition.z < -0.975f) { for (int i = 0; i < armRenderers.Length; i++) { armRenderers[i].enabled = false; } }
        }
        else { arms.localPosition = Vector3.Lerp(arms.localPosition, new Vector3(0, 0, 0f), Time.deltaTime * currentShowHideSpeed); }
    }
    public void Show(float speed){
        hidden = false;
        currentShowHideSpeed = speed;
        for (int i = 0; i < armRenderers.Length; i++) { armRenderers[i].enabled = true; }
    }
    public void Hide(float speed){
        hidden = true;
        currentShowHideSpeed = speed;
    }
}
