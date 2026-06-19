using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpreadCounter : MonoBehaviour
{
    public TextMeshProUGUI spreadText;
    private void Awake()
    {
        Cache.spreadCounter = this;
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }
    private void Update()
    {
        spreadText.text = Firearm.globalCurrentInaccuracy.ToString("F2");
    }
    private void OnEnable()
    {
        spreadText.gameObject.SetActive(true);
    }
    private void OnDisable()
    {
        spreadText.gameObject.SetActive(false);
    }
}
