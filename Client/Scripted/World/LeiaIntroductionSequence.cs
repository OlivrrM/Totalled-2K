using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeiaIntroductionSequence : MonoBehaviour
{
    public Robot runningLeia;
    public RobotHealth runningLeiaHealth;
    public Transform runningPoint;
    public float leiaDespawnTime;

    public Robot[] spawnedLeias;
    public void Trigger()
    {
        StartCoroutine(RunSequence());
    }
    IEnumerator RunSequence()
    {
        runningLeia.SetDestination(runningPoint.position);
        yield return new WaitForSeconds(leiaDespawnTime);
        if (runningLeiaHealth.health > 0f){
            runningLeia.gameObject.SetActive(false);
        }
    }
    public void TriggerPartTwo()
    {
        StartCoroutine(SpawnSequence());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) && !Terminal.terminalActive)
        {
            StartCoroutine(SpawnSequence());
        }
    }
    IEnumerator SpawnSequence()
    {
        for (int i = 0; i < spawnedLeias.Length; i++){
            spawnedLeias[i].gameObject.SetActive(true);
            spawnedLeias[i].manualControl = true;
            spawnedLeias[i].currentActionState = Totalled.RobotActionState.Chasing;
            spawnedLeias[i].Agro();
            spawnedLeias[i].lastKnownTargetPos = Cache.surfCharacter.transform.position;
            spawnedLeias[i].currentAgroRadius = 100f;
            spawnedLeias[i].currentForceAgroRadius = 100f;
            spawnedLeias[i].SetDestination(Cache.surfCharacter.transform.position);
            spawnedLeias[i].distanceFromTarget = Vector3.Distance(transform.position, Cache.surfCharacter.transform.position);
        }
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < spawnedLeias.Length; i++){
            spawnedLeias[i].manualControl = false;
        }
    }
}
