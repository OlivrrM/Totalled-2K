using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedCommand : MonoBehaviour
{
    public float delay;
    public string command;
    public bool destroyOnExecute;
    public void SetCommand(string cmd, string spaceIdentifier)
    {
        command = cmd.Replace(spaceIdentifier, " ");
    }
    private void Update()
    {
        if (delay > 0)
        {
            delay -= Time.deltaTime;
            if (delay <= 0)
            {
                Cache.terminal.PreExecute(command);
                if (destroyOnExecute) { Destroy(gameObject); }
            }
        }
    }
}
