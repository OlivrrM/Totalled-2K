using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadAssets : MonoBehaviour
{
    public string loadInstanceName;
    private void Start()
    {
        Cache.gameScripts = GameObject.Find("GameScripts");
        StartCoroutine(EnterInstance());
    }
    IEnumerator EnterInstance()
    {
        yield return new WaitForEndOfFrame();
        Cache.instanceManagement.LoadInstance(loadInstanceName);
        OnEnterInstance();
    }
    void OnEnterInstance()
    {
        Cache.references.binds = InputManager.inputBinds.savedBinds.binds;
    }
}
