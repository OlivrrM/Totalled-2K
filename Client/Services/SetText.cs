using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetText : MonoBehaviour
{
    public TextMeshProUGUI txt;
    public void Set(string text, string spaceIdentifier)
    {
        txt.text = text.Replace(spaceIdentifier, " ");
    }
}
