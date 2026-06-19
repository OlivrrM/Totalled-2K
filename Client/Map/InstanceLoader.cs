using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InstanceLoader : MonoBehaviour
{
    public void AsyncInstanceLoad(string instanceName){
        if (!Utilities.IsSceneLoaded(instanceName)){
            Cache.instanceManagement.AsyncLoadInstanceAdditive(instanceName);
        }
    }
    public void AsyncInstanceUnload(string instanceName){
        if (Utilities.IsSceneLoaded(instanceName)){
            SceneManager.UnloadSceneAsync(instanceName);
        }
    }
}
