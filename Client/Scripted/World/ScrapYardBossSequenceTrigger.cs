using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class ScrapYardBossSequenceTrigger : MonoBehaviour
{
    public Transform jumpPos;
    public Robot robot;
    public TARMRobotArmAnimator armAnimator0;
    public TARMRobotArmAnimator armAnimator1;

    Vector2 defaultAgroRadius;

    public GameObject shatteredWindow;
    public GameObject intactWindow;
    public List<Rigidbody> objectsToExplode = new List<Rigidbody>();

    public PlaySound shatterSfx;

    public float windowShatterForce;

    public GameObject disableRobotTrigger;
    public GameObject enableRobotTrigger;

    public Animator ChandelierAnimator;
    public Animator bossChairAnimator;

    public DestroyChildrenOnDestroyableStaticDeDeath radioToDestroy;
    public void Start(){
        robot.manualControl = true;
        robot.gameObject.SetActive(false);
        defaultAgroRadius = new Vector2(robot.agroRadius, robot.forceAgroRadius);
        robot.agroRadius = 0f; robot.forceAgroRadius = 0f;
        shatteredWindow.SetActive(false);
        for (int i = 0; i < objectsToExplode.Count; i++){
            objectsToExplode[i].isKinematic = true;
        }
        for (int i = 0; i < shatteredWindow.transform.childCount; i++){
            objectsToExplode.Add(shatteredWindow.transform.GetChild(i).GetComponent<Rigidbody>());
            objectsToExplode[i].isKinematic = true;
        }
        armAnimator0.dontEffectPlayer = true;
        armAnimator1.dontEffectPlayer = true;
    }
    public void Trigger()
    {
        disableRobotTrigger.SetActive(false);
        enableRobotTrigger.SetActive(true);
        StartCoroutine(robot.PerformJump(jumpPos.position));
        robot.agroRadius = defaultAgroRadius.x; robot.forceAgroRadius=defaultAgroRadius.y;
        StartCoroutine(TriggerSequence());
    }
    IEnumerator TriggerSequence()
    {
        yield return new WaitForSeconds(robot.jumpDuration);
        robot.currentActionState = Totalled.RobotActionState.Attacking;
        intactWindow.SetActive(false);
        shatteredWindow.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        shatterSfx.Play();
        for (int i = 0; i < objectsToExplode.Count; i++){
            objectsToExplode[i].isKinematic = false;
            objectsToExplode[i].AddExplosionForce(windowShatterForce,robot.head.position, 5);
            objectsToExplode[i].gameObject.layer = 9;
        }
        ChandelierAnimator.SetBool("Sway", true);
        bossChairAnimator.SetBool("Start", true);
        robot.currentActionState = Totalled.RobotActionState.Chasing;
        radioToDestroy.Damage(new Damage { amount = 999f });
        robot.manualControl = false;
        yield return new WaitForSeconds(0.1f);
        armAnimator0.dontEffectPlayer = false;
        armAnimator1.dontEffectPlayer = false;
    }
    public void EnableRobot(){
        robot.gameObject.SetActive(true);
    }
    public void DisableRobot()
    {
        robot.gameObject.SetActive(false);
    }
}
