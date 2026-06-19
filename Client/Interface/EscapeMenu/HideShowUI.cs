using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HideShowUI : MonoBehaviour
{
    public float fixedBackScale;
    CanvasGroup canvasGroup;
    public float smoothness = 8f;
    public void Start()
    {
        if (canvasGroup == null) { canvasGroup = gameObject.GetComponent<CanvasGroup>(); }
    }
    public virtual void Hide()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    public virtual void Show()
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
    private void Update()
    {
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, canvasGroup.interactable ? 1f : 0f, Time.unscaledDeltaTime * smoothness);
    }
}
