using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveVcamManager : MonoBehaviour
{
    [HideInInspector] public CinemachineVirtualCamera currentVcam;
    private void Start()
    {
        Cache.cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
        Cache.activeVcamManager = this;
    }
    private void Update()
    {
        currentVcam = Cache.cinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCamera;
    }
}
