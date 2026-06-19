using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapYardTrainPass : MonoBehaviour
{
    public Transform train;

    public AudioSource sfx;
    public CinemachineImpulseSource camShakeImpusle;
    public Vector2 sfxPosXClampAB;

    public float trainDelayTime;
    public float trainSpeed;
    public float trainTime;
    float trainCurrentTime;

    bool triggered;

    Vector3 defaultTrainPos;
    private void Start()
    {
        train.gameObject.SetActive(false);
        defaultTrainPos = train.transform.position;
    }
    public void Trigger()
    {
        train.transform.position = defaultTrainPos;
        trainCurrentTime = 0f;
        triggered = false;
        sfx.Play();
        StartCoroutine(DelayedTrigger());
    }
    IEnumerator DelayedTrigger()
    {
        yield return new WaitForSeconds(trainDelayTime);
        triggered = true;
        train.gameObject.SetActive(true);
    }
    private void Update()
    {
        if (triggered){
            sfx.transform.position = new Vector3(Mathf.Clamp(train.position.x, sfxPosXClampAB.x, sfxPosXClampAB.y), train.position.y,train.position.z);
            camShakeImpusle.transform.position = new Vector3(Mathf.Clamp(train.position.x, sfxPosXClampAB.x, sfxPosXClampAB.y), train.position.y,train.position.z);
            train.position += -train.up * trainSpeed * Time.deltaTime;
            trainCurrentTime += Time.deltaTime;
            if (trainCurrentTime >= trainTime){
                triggered = false;
                train.gameObject.SetActive(false);
            }
        }
    }
    private void FixedUpdate()
    {
        if (triggered)
        {
            camShakeImpusle.GenerateImpulse();
        }
    }
}
