using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanceObjectSaveManager : MonoBehaviour
{
    public Dictionary<int, bool> objectStates = new Dictionary<int, bool>();
    public Dictionary<int,SaveObjectStateOverInstance> activeObjects = new Dictionary<int, SaveObjectStateOverInstance>();
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        activeObjects.Clear();
    }
    private void OnEnable()
    {
        Cache.instanceManagement.OnInstanceLoadEvent += OnInstanceLoad;
    }
    private void OnDisable()
    {
        Cache.instanceManagement.OnInstanceLoadEvent -= OnInstanceLoad;
    }
    public void OnInstanceLoad(bool success, string instanceName)
    {
        //print(gameObject.name + "   SUCCESS");
        if (success)
        {
            if (objectStates.Count <= 0)
            {
                foreach (KeyValuePair<int, SaveObjectStateOverInstance> kvp in activeObjects)
                {
                    objectStates.Add(kvp.Key, true);
                }
            }
            else
            {
                foreach (KeyValuePair<int, bool> kvp in objectStates)
                {
                    foreach (KeyValuePair<int, SaveObjectStateOverInstance> obj in activeObjects)
                    {
                        if (obj.Key == kvp.Key)
                        {
                            //print(obj.Key);
                            if (!kvp.Value)
                            {
                                Destroy(obj.Value.gameObject);
                            }
                        }
                    }
                }
            }
        }
    }
}
