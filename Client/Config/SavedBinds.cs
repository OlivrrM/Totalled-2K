using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Totalled;

[System.Serializable]
public class SavedBinds
{
    public Dictionary<string, List<Keybind>> binds;
    public List<CommandExecutionBind> commandExecutionBinds;
}
