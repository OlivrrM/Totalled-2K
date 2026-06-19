using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationClipLocalAnchor : MonoBehaviour
{
    public Transform playerGraphics;

    public Transform headReferencePoint;

    public WorldSpacePositionOffsetManager worldSpacePositionOffsetManager;
    private void Awake()
    {
        worldSpacePositionOffsetManager.AddValue("ClipOffsetFixer", Vector3.zero);
    }
    private void Update()
    {
        //playerGraphics.localPosition = Vector3.zero;
        //playerGraphics.localPosition -= Cache.playerAnimator.animators[0].transform.localPosition;
    }
}
