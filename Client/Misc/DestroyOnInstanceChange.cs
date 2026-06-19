using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnInstanceChange : MonoBehaviour
{
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
        if (success)
        {
            Destroy(gameObject);
        }
    }
}
