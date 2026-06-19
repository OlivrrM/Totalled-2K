using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SaveObjectStateOverInstance : MonoBehaviour
{
    public InstanceObjectSaveManager instanceObjectSaveManager;
    [SerializeField] int objectID;

    bool initialized;
    private void OnValidate()
    {
        objectID = Guid.NewGuid().GetHashCode();
    }
    private void Awake()
    {
        if (instanceObjectSaveManager == null){
            GameObject instanceObjectSaveManagerGO = GameObject.Find($"{gameObject.scene.name}_InstanceObjectSaveManager");
            if (instanceObjectSaveManagerGO == null){
                GameObject newManager = new GameObject($"{gameObject.scene.name}_InstanceObjectSaveManager");
                newManager.AddComponent<InstanceObjectSaveManager>();
                instanceObjectSaveManager = newManager.GetComponent<InstanceObjectSaveManager>();
            }
            else{
                instanceObjectSaveManager = instanceObjectSaveManagerGO.GetComponent<InstanceObjectSaveManager>();
            }
        }
    }
    void Start()
    {
        instanceObjectSaveManager.objectStates.Add(objectID, this);
        initialized = true;
    }
    private void OnDestroy()
    {
        if (initialized){
            instanceObjectSaveManager.objectStates[objectID] = false;
        }
    }
}
