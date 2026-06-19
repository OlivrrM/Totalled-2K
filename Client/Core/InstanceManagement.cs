using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using TMPro;

public class InstanceManagement : MonoBehaviour
{
    public GameObject loadingIndicator;
    public GameObject asyncLoadingIndicator;

    // Thread hack
    [HideInInspector] public bool GlobalThreadStartLoadingScene;
    [HideInInspector] public string GlobalThreadLoadSceneName;

    public event Action<bool,string> OnInstanceLoadEvent;
    public event Action<bool,string> OnAsyncInstanceLoadEvent;
    private void Awake()
    {
        Cache.instanceManagement = this;
        asyncLoadingIndicator.SetActive(false);
        DontDestroyOnLoad(this);
    }
    public void LoadInstance(string instanceName)
    {
        GlobalThreadStartLoadingScene = false;
        loadingIndicator.SetActive(true);
        StartCoroutine(Load(instanceName));
    }
    public void AsyncLoadInstanceAdditive(string instanceName)
    {
        asyncLoadingIndicator.SetActive(true);
        StartCoroutine(AsyncLoad(instanceName));
    }
    IEnumerator AsyncLoad(string instanceName)
    {
        AsyncOperation asyncLoad = null;
        if (Utilities.DoesSceneExist(instanceName) || instanceName == "this")
        {
            if (instanceName == "this") { asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name, LoadSceneMode.Additive); }
            else { asyncLoad = SceneManager.LoadSceneAsync(instanceName, LoadSceneMode.Additive); }
        }
        else
        {
            Debug.LogError($"Attempted to async load instance '{instanceName}', however no such instance could be found");
            asyncLoadingIndicator.SetActive(false);
            OnAsyncInstanceLoad(false, instanceName);
        }

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        asyncLoadingIndicator.SetActive(false);
        OnAsyncInstanceLoad(true,instanceName);
    }
    IEnumerator Load(string instanceName) // Utilities.DoesSceneExist(instanceName) needs to be ran in Main Thread
    {
        yield return new WaitForEndOfFrame();
        if (Utilities.DoesSceneExist(instanceName) || instanceName=="this")
        {
            if (instanceName == "this") { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
            else { SceneManager.LoadScene(instanceName); }
            OnInstanceLoad(true, instanceName);
        }
        else
        {
            Debug.LogError($"Attempted to load instance '{instanceName}', however no such instance could be found");
            loadingIndicator.SetActive(false);
            OnInstanceLoad(false,instanceName);
        }
    }
    public void OnInstanceLoad(bool success,string instanceName)
    {
        OnInstanceLoadEvent?.Invoke(success,instanceName);
    }
    public void OnAsyncInstanceLoad(bool success, string instanceName)
    {
        OnAsyncInstanceLoadEvent?.Invoke(success, instanceName);
    }
    private void Update()
    {
        if (GlobalThreadStartLoadingScene)
        {
            LoadInstance(GlobalThreadLoadSceneName);
            GlobalThreadStartLoadingScene = false;
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        loadingIndicator.SetActive(false);
    }
}
