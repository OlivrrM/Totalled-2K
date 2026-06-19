using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class UnloadInstanceClipBrush : ClipBrush
{
    public string targetInstance;
    public bool disableOnTrigger;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            SceneManager.UnloadSceneAsync(targetInstance);
            if (disableOnTrigger) { gameObject.SetActive(false); }
        }
    }
}
