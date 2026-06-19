using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandSwitch : MonoBehaviour
{
    bool on;
    public string onCommand;
    public string offCommand;
    public void SetCommand(bool on,string command,string spaceIdentifier)
    {
        if (on) { onCommand = command.Replace(spaceIdentifier, " "); }
        else { offCommand = command.Replace(spaceIdentifier, " "); }
    }
    public void Switch()
    {
        on = !on;
        if (on) { Cache.terminal.PreExecute(onCommand); }
        else { Cache.terminal.PreExecute(offCommand); }
    }
}
