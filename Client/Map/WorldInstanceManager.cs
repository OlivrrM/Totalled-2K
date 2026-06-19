using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldInstanceManager : MonoBehaviour
{
    private void OnEnable()
    {
        Cache.instanceManagement.OnInstanceLoadEvent += OnInstanceLoad;
    }
    private void OnDisable()
    {
        Cache.instanceManagement.OnInstanceLoadEvent -= OnInstanceLoad;
    }
    public void OnInstanceLoad(bool success, string instanceName) // Currently, world level needs to be loaded before world itself, due to lighting bug
    {
        if (success)
        {
            if (instanceName == "World") //Default to loading from the beginning at W0
            {
                SceneManager.LoadScene("World", LoadSceneMode.Single);
                SceneManager.LoadScene("W0", LoadSceneMode.Additive);
            }
            else
            {
                Regex regex = new Regex(@"^W\d+$"); // If target is world level, load World then target
                if (regex.IsMatch(instanceName))
                {
                    SceneManager.LoadScene("World", LoadSceneMode.Single);
                    SceneManager.LoadScene(instanceName, LoadSceneMode.Additive);
                }
            }
        }
    }
}
