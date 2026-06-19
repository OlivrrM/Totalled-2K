using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossInstanceEventCleaner : MonoBehaviour
{
    public List<GameObject> objectsToDisable = new List<GameObject>();
    public List<GameObject> objectsToEnable = new List<GameObject>();
    CleanupEventOnReloadedInstance cleanupEventOnReloadedInstance;
    private void Awake()
    {
        cleanupEventOnReloadedInstance = GameObject.Find($"InstanceScripts_{gameObject.scene.name}").GetComponent<CleanupEventOnReloadedInstance>();
        cleanupEventOnReloadedInstance.objectsToDisable = objectsToDisable;
        cleanupEventOnReloadedInstance.objectsToEnable = objectsToEnable;
    }
    public void MarkEventComplete()
    {
        cleanupEventOnReloadedInstance.MarkEventComplete();
    }
}
