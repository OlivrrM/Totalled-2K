using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoEndScreenTrigger : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    bool triggered;
    bool exitted;

    public Animator screenAnimator;
    public void Trigger()
    {
        triggered = true;
        Cache.walkSpeedManager.UpdateValue("DemoEnd", 0f);
        Cache.jumpHeightManagerScript.UpdateValue("DemoEnd", 0f);
        screenAnimator.SetBool("Start", true);
    }
    private void Start()
    {
        StartCoroutine(DelayedStart());
    }
    IEnumerator DelayedStart()
    {
        yield return new WaitForEndOfFrame();
        Cache.walkSpeedManager.AddValue("DemoEnd", 1f);
        Cache.jumpHeightManagerScript.AddValue("DemoEnd", 1f);
    }
    private void Update()
    {
        if (triggered)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1f, Time.deltaTime);
            if (Input.GetKeyDown(KeyCode.Return)){
                triggered = false;
                Cache.walkSpeedManager.UpdateValue("DemoEnd", 1f);
                Cache.jumpHeightManagerScript.UpdateValue("DemoEnd", 1f);
            }
        }
        else
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0f, Time.deltaTime);
        }
    }
}
