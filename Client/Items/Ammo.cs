using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Ammo : MonoBehaviour
{
    public int amount;

    public CounterGraphicManager counterGraphicManager;
    public CounterGraphicManager currentClipCounterGraphicManager;

    public CanvasGroup crosshairCanvasGroup;
    private void Start()
    {
        Cache.ammo = this;
        counterGraphicManager.SetCounter(amount);
    }
    public void ChangeAmmo(int change)
    {
        amount += change;
        counterGraphicManager.SetCounter(amount);
    }
    public void SetAmmo(int amount)
    {
        this.amount = amount;
        counterGraphicManager.SetCounter(amount);
    }
    public void NoAmmoAvailableIndication()
    {
        counterGraphicManager.DenyCounter();
    }
    public void NoAmmoInClipIndication()
    {
        currentClipCounterGraphicManager.DenyCounter();
        crosshairCanvasGroup.alpha = 0.25f;
        StartCoroutine(NoAmmoInClipIndicatorCanvasGroupShow());
    }
    IEnumerator NoAmmoInClipIndicatorCanvasGroupShow()
    {
        yield return new WaitForSeconds(0.2f);
        crosshairCanvasGroup.alpha = 1f;
    }
}
