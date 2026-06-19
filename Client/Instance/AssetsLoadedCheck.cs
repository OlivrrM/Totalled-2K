using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AssetsLoadedCheck : MonoBehaviour
{
    private void Start()
    {
        if (Cache.references == null) { SceneManager.LoadScene("LoadAssets"); }
    }
}
