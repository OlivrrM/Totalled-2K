using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CleanupEventOnReloadedInstance : MonoBehaviour
{
    [HideInInspector] public List<GameObject> objectsToDisable = new List<GameObject>();
    [HideInInspector] public List<GameObject> objectsToEnable = new List<GameObject>();
    public bool eventComplete { get; private set; }
    string thisInstanceName;
    string thisMainInstanceName;
    private void Start()
    {
        if (GameObject.Find(gameObject.name) != gameObject) { Destroy(gameObject); }
        else { 
            thisInstanceName = gameObject.scene.name;
            thisMainInstanceName = SceneManager.GetActiveScene().name;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        Cache.instanceManagement.OnAsyncInstanceLoadEvent += OnAsyncInstanceLoad;
        Cache.instanceManagement.OnInstanceLoadEvent += OnInstanceLoad;
    }
    private void OnDisable()
    {
        Cache.instanceManagement.OnAsyncInstanceLoadEvent -= OnAsyncInstanceLoad;
        Cache.instanceManagement.OnInstanceLoadEvent -= OnInstanceLoad;
    }
    public void MarkEventComplete()
    {
        eventComplete = true;
    }
    public void OnAsyncInstanceLoad(bool success, string instanceName)
    {
        if (success && eventComplete && instanceName == thisInstanceName)
        {
            for (int i = 0; i < objectsToDisable.Count; i++){
                objectsToDisable[i].SetActive(false);
            }
            for (int i = 0; i < objectsToEnable.Count; i++){
                objectsToEnable[i].SetActive(true);
            }
        }
    }
    public void OnInstanceLoad(bool success, string instanceName)
    {
        if (success){
            Destroy(gameObject);
        }
    }
}
