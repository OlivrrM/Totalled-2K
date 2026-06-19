using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Email : MonoBehaviour
{
    public void Send(string recipient, string title,string body)
    {
        Application.OpenURL("mailto:" + recipient + "?subject=" + title + "&body=" + body);
    }
}
