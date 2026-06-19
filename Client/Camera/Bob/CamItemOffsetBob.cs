using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class CamItemOffsetBob : MonoBehaviour
{
    public Transform target;
    public Transform compareTargetRelativeTo; // Used when the target graphics local offset doesn't change, so a relative world space distance check from the handheld render pos is used instead
    Vector3 defaultOffset;

    public float bobAmount;
    public float bobSmoothness;
    Vector4 currentBob;
    Vector4 targetBob;
    private void Start()
    {
        defaultOffset = compareTargetRelativeTo != null ? compareTargetRelativeTo.InverseTransformPoint(target.position) : target.localPosition;
        Cache.cameraRotationOffsetManager.AddOffset(transform.name + "ItemOffset", new CameraRotationOffset { offset = Vector4.zero });
    }
    private void Update()
    {
        if (compareTargetRelativeTo != null) {
            Vector3 localPos = compareTargetRelativeTo.InverseTransformPoint(target.position);
            targetBob = new Vector4((localPos.y - defaultOffset.y) * -bobAmount, (localPos.x - defaultOffset.x) * bobAmount, 0, 0); 
        }
        else { targetBob = new Vector4((target.localPosition.y - defaultOffset.y) * -bobAmount, (target.localPosition.x - defaultOffset.x) * bobAmount, 0, 0); }
        currentBob = Vector4.Lerp(currentBob, targetBob, Time.deltaTime * bobSmoothness);

        Cache.cameraRotationOffsetManager.UpdateOffset(transform.name + "ItemOffset", new CameraRotationOffset { offset = currentBob });
    }
}
