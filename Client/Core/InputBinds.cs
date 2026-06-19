using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Totalled;
using System.IO;
using Newtonsoft.Json;

public class InputBinds
{
    //public Dictionary<string, List<Keybind>> binds = new Dictionary<string, List<Keybind>>();
    public SavedBinds savedBinds = new SavedBinds();

    public InputBinds()
    {
        
    }
    public void SetDefaultBinds()
    {

        savedBinds = new SavedBinds();

        savedBinds.binds = new Dictionary<string, List<Keybind>>();

        savedBinds.commandExecutionBinds = new List<CommandExecutionBind>();

        Cache.terminal.commandExecutionBinds = new List<CommandExecutionBind>();

        AddBind("Jump", KeyCode.Space, KeybindType.Key);

        AddBind("Vault", KeyCode.Space, KeybindType.Down);

        AddBind("MoveLeft", KeyCode.A, KeybindType.Key);
        AddBind("MoveRight", KeyCode.D, KeybindType.Key);
        AddBind("MoveForward", KeyCode.W, KeybindType.Key);
        AddBind("MoveBack", KeyCode.S, KeybindType.Key);

        AddBind("YawLeft", KeyCode.None, KeybindType.Key);
        AddBind("YawRight", KeyCode.None, KeybindType.Key);

        AddBind("Crouch", KeyCode.LeftControl, KeybindType.Key);

        AddBind("Stabilize", KeyCode.None, KeybindType.Key);

        AddBind("Sprint", KeyCode.LeftShift, KeybindType.Key);

        AddBind("Unequip", KeyCode.Q, KeybindType.Down);

        AddBind("MainAction", KeyCode.Mouse0, KeybindType.Key);
        AddBind("SecondaryAction", KeyCode.Mouse1, KeybindType.Key);

        AddBind("NoClip", KeyCode.N, KeybindType.Down);

        AddBind("ResetOrigin", KeyCode.R, KeybindType.Down);

        AddBind("Interact", KeyCode.E, KeybindType.Down);

        AddBind("Grenade", KeyCode.G, KeybindType.Down);

        AddBind("Focus", KeyCode.C, KeybindType.Key);

        AddBind("Reload", KeyCode.R, KeybindType.Down);

        AddBind("ToggleInterface", KeyCode.F1, KeybindType.Down);

        AddBind("Flashlight", KeyCode.F, KeybindType.Down);

        AddBind("Throw", KeyCode.Tab, KeybindType.Down);

        AddBind("Escape", KeyCode.Escape, KeybindType.Down);

        AddBind("Inspect", KeyCode.Mouse2, KeybindType.Down);


        AddBind("InventorySlotOne", KeyCode.Alpha1, KeybindType.Down);
        AddBind("InventorySlotTwo", KeyCode.Alpha2, KeybindType.Down);
        AddBind("InventorySlotThree", KeyCode.Alpha3, KeybindType.Down);
        AddBind("InventorySlotFour", KeyCode.Alpha4, KeybindType.Down);
        AddBind("InventorySlotFive", KeyCode.Alpha5, KeybindType.Down);
        AddBind("InventorySlotSix", KeyCode.Alpha6, KeybindType.Down);
        AddBind("InventorySlotSeven", KeyCode.Alpha7, KeybindType.Down);
        AddBind("InventorySlotEight", KeyCode.Alpha8, KeybindType.Down);
        AddBind("InventorySlotNine", KeyCode.Alpha9, KeybindType.Down);

        AddBind("Screenshot", KeyCode.Print, KeybindType.Down);
    }
    public void LoadBinds()
    {
        string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Save");
        string filePath = Path.Combine(directoryPath, "binds.config");

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            Cache.terminal.Print($"Save directory not found, new Save directory created");
        }

        if (!File.Exists(filePath))
        {
            SetDefaultBinds();
            File.WriteAllText(filePath, JsonConvert.SerializeObject(savedBinds, new JsonSerializerSettings { Formatting = Formatting.Indented }));
            Cache.terminal.Print($"binds.config not found, creating new with default binds at: {directoryPath}");
        }
        string fileContents = File.ReadAllText(filePath);
        savedBinds = JsonConvert.DeserializeObject<SavedBinds>(fileContents, new JsonSerializerSettings { Formatting = Formatting.Indented });
        Cache.terminal.commandExecutionBinds = savedBinds.commandExecutionBinds;
        Cache.terminal.Print("Loaded binds.config");
    }
    public void SaveBinds()
    {
        string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Save");
        string filePath = Path.Combine(directoryPath, "binds.config");

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            Cache.terminal.Print($"Save directory not found, new Save directory created");
        }

        if (!File.Exists(filePath))
        {
            SetDefaultBinds();
            File.WriteAllText(filePath, JsonConvert.SerializeObject(savedBinds, new JsonSerializerSettings { Formatting = Formatting.Indented }));
            Cache.terminal.Print($"binds.config not found, creating new with default binds at: {directoryPath}");
        }
        File.WriteAllText(filePath, JsonConvert.SerializeObject(savedBinds, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        Cache.terminal.Print("Saved binds to binds.config");
    }
    public void ResetAllBinds()
    {
        SetDefaultBinds();
        SaveBinds();
    }
    public void AddBind(string bindName,KeyCode setKeycode,KeybindType setKeybindType)
    {
        savedBinds.binds.Add(bindName,
           new List<Keybind>
           {
                new Keybind
                {
                    keycode = setKeycode,
                    keybindType = setKeybindType
                }
           }
       );
    }
    public void BindKey(KeyCode givenKeyCode,string bindName,KeybindType givenKeybindType)
    {
        if (savedBinds.binds.ContainsKey(bindName))
        {
            savedBinds.binds[bindName].Add(
                new Keybind
                {
                    keycode = givenKeyCode,
                    keybindType = givenKeybindType
                }
             );
        }
    }
    public void ClearBoundKey(Keybind setKeybind)
    {
        foreach (KeyValuePair<string,List<Keybind>> bind in savedBinds.binds)
        {
            for (int i = 0; i < bind.Value.Count; i++)
            {
                if (bind.Value[i].keycode == setKeybind.keycode && bind.Value[i].keybindType == setKeybind.keybindType)
                {
                    bind.Value.RemoveAt(i);
                }
            }
        }
    }
    /*
    public bool IsBindActive(string bindName, KeybindType fixedBindTypeCheck = KeybindType.None)
    {
        if (binds.ContainsKey(bindName))
        {
            bool isActive = false;
            for (int i = 0; i < binds[bindName].Count; i++)
            {
                Keybind keybind = binds[bindName][i];
                if (fixedBindTypeCheck != KeybindType.None) { keybind.keybindType = fixedBindTypeCheck; }
                switch (keybind.keybindType)
                {
                    case KeybindType.Down:
                        if (Input.GetKeyDown(keybind.keycode)) { isActive = true; }
                        break;
                    case KeybindType.Key:
                        if (Input.GetKey(keybind.keycode)) { isActive = true; }
                        break;
                    case KeybindType.Up:
                        if (Input.GetKeyUp(keybind.keycode)) { isActive = true; }
                        break;
                }
                if (isActive) { return true; }
            }
            return isActive;
        }
        else
        {
            Debug.LogWarning($"Tried to check none existant bind '{bindName}'");
            return false;
        }
    }
    */
    public bool IsBindActive(string bindName, KeybindType fixedBindTypeCheck = KeybindType.None)
    {
        if (!savedBinds.binds.ContainsKey(bindName))
        {
            Debug.LogWarning($"Tried to check nonexistent bind '{bindName}'");
            return false;
        }

        var bindList = savedBinds.binds[bindName];
        for (int i = 0; i < bindList.Count; i++)
        {
            Keybind keybind = bindList[i];
            KeybindType bindTypeToCheck = fixedBindTypeCheck != KeybindType.None ? fixedBindTypeCheck : keybind.keybindType;

            // Determine if the keybind is active based on its type
            bool isActive = bindTypeToCheck switch
            {
                KeybindType.Down => Input.GetKeyDown(keybind.keycode),
                KeybindType.Key => Input.GetKey(keybind.keycode),
                KeybindType.Up => Input.GetKeyUp(keybind.keycode),
                _ => false
            };

            if (isActive)
            {
                return true;
            }
        }
        return false;
    }
}
