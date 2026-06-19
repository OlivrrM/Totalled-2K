using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetLoadedInstancesOnRespawn : MonoBehaviour
{
    public List<string> instancesToLoadOnRespawn = new List<string>(); // Scene names to load on respawn

    private void Awake()
    {
        Cache.setLoadedInstancesOnRespawn = this;
    }

    public void LoadInstances()
    {
        StartCoroutine(SyncInstancesCoroutine(instancesToLoadOnRespawn));
    }

    private IEnumerator SyncInstancesCoroutine(List<string> desiredSceneNames)
    {
        HashSet<string> desiredSet = new HashSet<string>(desiredSceneNames);

        // Unload scenes that aren't in the desired set
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene loadedScene = SceneManager.GetSceneAt(i);
            if (!desiredSet.Contains(loadedScene.name) && loadedScene.name != "DontDestroyOnLoad" && loadedScene.name != "World")
            {
                AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(loadedScene);
                while (!unloadOp.isDone)
                    yield return null;
            }
        }

        // Load scenes that are missing
        foreach (string sceneName in desiredSceneNames)
        {
            Scene currentlyLoaded = SceneManager.GetSceneByName(sceneName);
            if (!currentlyLoaded.isLoaded)
            {
                AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                while (!loadOp.isDone)
                    yield return null;
            }
        }
    }

    public void SetInstancesToLoadOnRespawn(List<string> sceneNames)
    {
        instancesToLoadOnRespawn.Clear();
        instancesToLoadOnRespawn.AddRange(sceneNames);
    }
}