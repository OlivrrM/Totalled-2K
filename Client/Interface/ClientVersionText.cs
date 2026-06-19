using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClientVersionText : MonoBehaviour
{
    public TextMeshProUGUI text;
    private void Start()
    {
        if (!string.IsNullOrEmpty(ClientVersionManager.version) && ClientVersionManager.version != "")
        {
            text.text = ClientVersionManager.version;
        }
        else
        {
            text.text = "Unknown Client Version";
        }
    }
}
