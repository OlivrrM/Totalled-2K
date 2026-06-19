using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargetInfo : MonoBehaviour
{
    public static Vector3 pos;
    private void Update()
    {
        pos = Cache.mainCam.transform.position + new Vector3(0f, -(Cache.surfCharacter.ViewOffset.y / 2f), 0f);
    }
}
