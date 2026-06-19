using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandOnCollision : MonoBehaviour
{
    public GameObject target;
    public string command;
    public bool destroyOnExecute;
    public void SetCommand(string cmd, string spaceIdentifier)
    {
        command = cmd.Replace(spaceIdentifier, " ");
    }
    public void SetTarget(string name)
    {
        target = GameObject.Find(name);
    }
    public void SetAnyTarget()
    {
        target = null;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (target == null){
            Cache.terminal.PreExecute(command);
            if (destroyOnExecute) { Destroy(gameObject); }
        }
        else{
            if (other.gameObject == target){
                Cache.terminal.PreExecute(command);
                if (destroyOnExecute) { Destroy(gameObject); }
            }
        }
    }
}
