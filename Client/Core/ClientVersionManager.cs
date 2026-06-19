using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientVersionManager : MonoBehaviour
{
    public string setVersion;
    public static string version;
    private void Awake()
    {
        version = setVersion;
    }
}
