using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Escape : MonoBehaviour
{
    public static bool active;
    public bool lockEscapeMenu = false;
    public float escapeTimeScaleSpeed;
    public PostFxVolumeManager escapePostFxManager;
    float currentTimeScaleMultiplier;

    public InterfaceElementShowHideManager[] interfaceElementShowHideManagers;

    public RectTransform topLine;
    public RectTransform topLine1;
    public RectTransform top;
    public RectTransform bottomLine;
    public RectTransform bottomLine1;
    public RectTransform bottom;
    public RectTransform backFadeRect;
    public RectTransform backFadeRect1;
    public TextMeshProUGUI pageTitle;

    public TextMeshProUGUI titleText;

    public Animator backAnimator;

    public Transform currentPage;

    List<RectTransform> currentOptions = new List<RectTransform>();

    public float fixedBackScale;

    public PlaySound openSfx;
    public PlaySound closeSfx;

    private void Awake()
    {
        TimeScaleManager.AddValue("Escape", 1f);
        CursorManager.AddActivator("Escape", false);
        backAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        List<Transform> family = Utilities.GetAllChildTransforms(currentPage);
        for (int i = 0; i < family.Count; i++) {
            //Image image = family[i].GetComponent<Image>();
            //if (image != null) { image.raycastTarget = false; }
            HideShowUI hideShowUI = family[i].GetComponent<HideShowUI>();
            if (hideShowUI != null) { hideShowUI.Start(); hideShowUI.Hide(); }
        }
        OpenPage(currentPage);
        HideCurrentPage();
        backAnimator.SetBool("Hide", true);
        Cache.escape = this;
    }
    public void Toggle()
    {
        active = !active;
        if (active) { escapePostFxManager.Enable(5f); }
        else { escapePostFxManager.Disable(5f); }
        CursorManager.UpdateActivator("Escape", active);
        for (int i = 0; i < interfaceElementShowHideManagers.Length; i++) {
            interfaceElementShowHideManagers[i].hidden = active; }
        Cache.inventoryUIManager.timeSinceLastEquip = active ? 3f : Cache.inventoryUIManager.timeSinceLastEquip;
        backAnimator.SetBool("Hide", !active);
        if (active) { ShowCurrentPage(); openSfx.Play(); }
        else { HideCurrentPage(); Cache.configSettings.SetSettingsOnExitPage();closeSfx.Play(); }

        if (active) { ConfigSave.edittingConfig = Utilities.DeepCopy(ConfigSave.config); }
        if (!active && (!Utilities.AreObjectsEqual(ConfigSave.edittingConfig, ConfigSave.config))) { ConfigSave.config = ConfigSave.edittingConfig; ConfigSave.Save(); }
    }
    private void OnApplicationQuit()
    {
        if (!Utilities.AreObjectsEqual(ConfigSave.edittingConfig, ConfigSave.config)){
            ConfigSave.config = ConfigSave.edittingConfig;
            ConfigSave.Save();
        }
    }
    public void HideCurrentPage()
    {
        for (int i = 0; i < currentPage.childCount; i++) {
            //Image image = currentPage.GetChild(i).GetComponent<Image>();
            //if (image != null) { image.raycastTarget = false; }
            HideShowUI hideShowUI = currentPage.GetChild(i).GetComponent<HideShowUI>();
            if (hideShowUI != null) { hideShowUI.Hide(); }
        }
        if (Cache.configSettings != null) { Cache.configSettings.SetSettingsOnExitPage(); }
    }
    public void ShowCurrentPage()
    {
        for (int i = 0; i < currentPage.childCount; i++) {
            //Image image = currentPage.GetChild(i).GetComponent<Image>();
            //if (image != null) { image.raycastTarget = true; }
            HideShowUI hideShowUI = currentPage.GetChild(i).GetComponent<HideShowUI>();
            if (hideShowUI != null) { hideShowUI.Show(); }
        }
    }
    public void OpenPage(Transform target)
    {
        currentOptions.Clear();
        for (int i = 0; i < currentPage.childCount; i++) {
            //Image image = currentPage.GetChild(i).GetComponent<Image>();
            //if (image != null) { image.raycastTarget = false; }
            HideShowUI hideShowUI = currentPage.GetChild(i).GetComponent<HideShowUI>();
            if (hideShowUI != null) { hideShowUI.Hide(); }
        }
        currentPage = target;
        for (int i = 0; i < currentPage.childCount; i++) {
            //Image image = currentPage.GetChild(i).GetComponent<Image>();
            //if (image != null) { image.raycastTarget = true; }
            HideShowUI hideShowUI = currentPage.GetChild(i).GetComponent<HideShowUI>();
            if (hideShowUI != null) {
                hideShowUI.Show();
                currentOptions.Add(hideShowUI.GetComponent<RectTransform>());
            }
        }
        TextMeshProUGUI pageTitleText = target.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (pageTitleText != null) { pageTitle.text = pageTitleText.text; }
        else { pageTitle.text = "Menu"; }

        HideShowUI targetHideShowUI = target.GetComponent<HideShowUI>();
        if (targetHideShowUI != null) { fixedBackScale = targetHideShowUI.fixedBackScale; }
        else { fixedBackScale = 0f; }
    }
    public void MainMenu()
    {
        Toggle();
        Cache.instanceManagement.LoadInstance("Dev");
    }
    public void Exit()
    {
        Application.Quit();
    }
    private void Update()
    {
        if (InputManager.GetEscapeKeyDown() && !lockEscapeMenu) { Toggle(); }

        currentTimeScaleMultiplier = Mathf.Lerp(currentTimeScaleMultiplier, Utilities.BoolToInt(!active), Time.unscaledDeltaTime * escapeTimeScaleSpeed);
        TimeScaleManager.UpdateValue("Escape", currentTimeScaleMultiplier);

        titleText.color = Color.Lerp(titleText.color, active ? Utilities.SetColorAlpha(titleText.color, 1f) : Utilities.SetColorAlpha(titleText.color, 0f), Time.unscaledDeltaTime * 10);

        if (currentOptions.Count>0){
            float scale = fixedBackScale == 0 ? Mathf.Clamp((currentOptions[0].anchoredPosition.y - currentOptions[currentOptions.Count - 1].anchoredPosition.y) * 0.005f, 0.2f, 2f) : fixedBackScale;
            backFadeRect.localScale = new Vector3(Mathf.Lerp(backFadeRect.localScale.x,scale,Time.unscaledDeltaTime * 10f), 1f, 1f);
            backFadeRect1.localScale = new Vector3(Mathf.Lerp(backFadeRect1.localScale.x, scale, Time.unscaledDeltaTime * 10f), 1f, 1f);
            /*
            topLine.anchoredPosition = new Vector2(topLine.anchoredPosition.x, top.rect.position.x);
            topLine1.anchoredPosition = new Vector2(topLine1.anchoredPosition.x, top.rect.position.x);
            bottomLine.anchoredPosition = new Vector2(bottomLine.anchoredPosition.x, bottom.rect.position.x);
            bottomLine1.anchoredPosition = new Vector2(bottomLine1.anchoredPosition.x, bottom.rect.position.x);
            */
            topLine.position = new Vector2(topLine.position.x, top.position.y);
            topLine1.position = new Vector2(topLine1.position.x, top.position.y);
            bottomLine.position = new Vector2(bottomLine.position.x, bottom.position.y);
            bottomLine1.position = new Vector2(bottomLine1.position.x, bottom.position.y);
        }
        pageTitle.transform.position = new Vector2(pageTitle.transform.parent.position.x, topLine.position.y + 50f);

        if (Terminal.terminalActive || Escape.active) { AudioListener.pause = true; } //TEMP. This needs to be its own system
        else { AudioListener.pause = false; }
    }
}
