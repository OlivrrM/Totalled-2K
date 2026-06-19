using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InterfaceHoverSfx : MonoBehaviour, IPointerEnterHandler 
{
    public static PlaySound sfx;
    CanvasGroup canvasGroup;
    void Start()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (sfx == null) { sfx = GameObject.Find("InterfaceHoverSfx").GetComponent<PlaySound>(); }
    }
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (canvasGroup.alpha>0.5f) { sfx.Play(); }
    }
}
