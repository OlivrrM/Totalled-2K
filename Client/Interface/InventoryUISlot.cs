using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUISlot : MonoBehaviour
{
    [HideInInspector] public RectTransform rectTransform;

    bool enabled;

    public ImageTransparency[] images;
    public TmpTextTransparency[] texts;
    public ImageTransparency iconTransparency;

    public RectTransform weaponIcon;
    public Vector2 weaponDisablePos;
    Vector2 weaponEnablePos;
    public Quaternion weaponDisableRot;
    Quaternion weaponEnableRot;
    public Vector2 weaponDisableScale;
    Vector2 weaponEnableScale;

    public TextMeshProUGUI numberText;
    public Vector2 numberDisablePos;
    Vector2 numberEnablePos;
    public TMP_FontAsset numberDisableMat;
    public TMP_FontAsset numberEnableMaterial;
    public Vector2 numberTextDisableScale;
    Vector2 numberTextEnableScale;

    [HideInInspector] public float targetX;
    private void Awake()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();

        weaponEnablePos = weaponIcon.anchoredPosition;
        weaponEnableRot = weaponIcon.localRotation;

        numberEnablePos = numberText.rectTransform.anchoredPosition;
        //numberEnableMaterial = numberText.font;

        numberTextEnableScale = numberText.rectTransform.localScale;

        weaponEnableScale = weaponIcon.localScale;

        //Disable();
    }
    public void Enable()
    {
        enabled = true;
        for (int i = 0; i < images.Length; i++)
        {
            images[i].SetToDefaultTransparency();
        }
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].SetToDefaultTransparency();
        }
        iconTransparency.SetTransparency(1f);
        numberText.font = numberEnableMaterial;
    }
    public void Disable()
    {
        enabled = false;
        for (int i = 0; i < images.Length; i++)
        {
            images[i].SetTransparency(0f);
        }
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].SetTransparency(0f);
        }
        iconTransparency.SetTransparency(0.8f);
        numberText.font = numberDisableMat;
    }
    private void Update()
    {
        float speed = images[0].smoothness;
        if (enabled)
        {
            weaponIcon.anchoredPosition = Vector2.Lerp(weaponIcon.anchoredPosition, weaponEnablePos, Time.deltaTime * speed);
            weaponIcon.localRotation = Quaternion.Lerp(weaponIcon.localRotation, weaponEnableRot, Time.deltaTime * speed);
            weaponIcon.localScale = Vector2.Lerp(weaponIcon.localScale, weaponEnableScale, Time.deltaTime * speed);

            numberText.rectTransform.anchoredPosition = Vector2.Lerp(numberText.rectTransform.anchoredPosition, numberEnablePos, Time.deltaTime * speed);
            numberText.rectTransform.localScale = Vector2.Lerp(numberText.rectTransform.localScale, numberTextEnableScale, Time.deltaTime * speed);
        }
        else
        {
            weaponIcon.anchoredPosition = Vector2.Lerp(weaponIcon.anchoredPosition, weaponDisablePos, Time.deltaTime * speed);
            weaponIcon.localRotation = Quaternion.Lerp(weaponIcon.localRotation, weaponDisableRot, Time.deltaTime * speed);
            weaponIcon.localScale = Vector2.Lerp(weaponIcon.localScale, weaponDisableScale, Time.deltaTime * speed);

            numberText.rectTransform.anchoredPosition = Vector2.Lerp(numberText.rectTransform.anchoredPosition, numberDisablePos, Time.deltaTime * speed);
            numberText.rectTransform.localScale = Vector2.Lerp(numberText.rectTransform.localScale, numberTextDisableScale, Time.deltaTime * speed);
        }

        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, new Vector2(rectTransform.anchoredPosition.x, -49.9f), Time.deltaTime * 6);
    }
}
