using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadInstanceClipBrush : ClipBrush
{
    public string targetInstance;
    public bool disableOnTrigger;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6){
            Load();
        }
    }
    public void Load()
    {
        if (gameObject.activeInHierarchy){
            Cache.instanceManagement.AsyncLoadInstanceAdditive(targetInstance);
            if (disableOnTrigger) { gameObject.SetActive(false); }
        }
    }
}
