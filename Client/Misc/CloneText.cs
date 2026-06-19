using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CloneText : MonoBehaviour
{
    public TextMeshProUGUI target;
    TextMeshProUGUI thisText;
    private void Start()
    {
        thisText = gameObject.GetComponent<TextMeshProUGUI>();
    }
    private void Update()
    {
        thisText.text = target.text;
        thisText.color = new Color(thisText.color.r, thisText.color.g, thisText.color.b, target.color.a);
        thisText.transform.localScale = target.transform.localScale;
    }
}
