using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideUI : MonoBehaviour
{
    /*
    public GameObject interfaceRoot;
    private void Update()
    {
        if (InputManager.GetToggleInterfaceKeyDown()){
            interfaceRoot.SetActive(!interfaceRoot.active);
        }
    }
    */
    public CanvasGroup canvasGroup;
    [HideInInspector] public bool hidden;
    private void Start()
    {
        Cache.hideUi = this;
    }
    private void Update()
    {
        if (InputManager.GetToggleInterfaceKeyDown()){
            hidden = !hidden;
        }
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, hidden ? 0f : 1f, Time.unscaledDeltaTime * 10f);
    }
}
