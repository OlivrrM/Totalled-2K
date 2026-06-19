using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FallingShake : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;

    public float shakeAmount;
    public float shakeSpeed;

    public float maxDownwardsVelocity;
    public float minDownwardsVelocity;

    CinemachineBasicMultiChannelPerlin shake;

    public float shakeRegainSpeed;

    float amplitudeTarget;
    float frequencyTarget;

    public float shakeSmoothness;
    private void Start()
    {
        shake = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    private void Update()
    {
        if (Cache.moveData.Velocity.y < 0){
            amplitudeTarget = Mathf.Clamp((Mathf.Clamp(Cache.moveData.Velocity.y, maxDownwardsVelocity, 0) - minDownwardsVelocity), -Mathf.Infinity, 0) * shakeAmount;
            frequencyTarget = Mathf.Clamp((Mathf.Clamp(Cache.moveData.Velocity.y, maxDownwardsVelocity, 0) - minDownwardsVelocity), -Mathf.Infinity, 0) * shakeSpeed;
        }
        else{
            amplitudeTarget = Mathf.Lerp(amplitudeTarget, 0, Time.deltaTime * shakeRegainSpeed);
            frequencyTarget = Mathf.Lerp(frequencyTarget, 0, Time.deltaTime * shakeRegainSpeed);
        }
        shake.m_AmplitudeGain = Mathf.Lerp(shake.m_AmplitudeGain, amplitudeTarget, Time.deltaTime * shakeSmoothness);
        shake.m_FrequencyGain = Mathf.Lerp(shake.m_FrequencyGain, amplitudeTarget, Time.deltaTime * shakeSmoothness);
    }
}
