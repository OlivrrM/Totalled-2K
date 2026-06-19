using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamShakeImpulseOnStart : MonoBehaviour
{
    public CinemachineImpulseSource impuse;
    private void Start()
    {
        impuse.GenerateImpulse();
    }
}
