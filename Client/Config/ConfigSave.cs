using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class ConfigSave : MonoBehaviour
{
    public static Config config;
    public static Config edittingConfig;
    public static string dir;
    private void Start()
    {
        Load();
    }
    public static void Load()
    {
        InputManager.inputBinds.LoadBinds();

        string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Save");
        string filePath = Path.Combine(directoryPath, "settings.config");

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            Cache.terminal.Print($"Save directory not found, new Save directory created at: {directoryPath}");
        }

        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, JsonConvert.SerializeObject(new Config(), new JsonSerializerSettings { Formatting = Formatting.Indented }));
            dir = filePath;
            Cache.terminal.Print($"settings.config not found, creating new with default settings at: {directoryPath}");
        }
        string fileContents = File.ReadAllText(filePath);
        //print(filePath);
        dir = filePath;
        config = JsonConvert.DeserializeObject<Config>(fileContents, new JsonSerializerSettings { Formatting = Formatting.Indented });
        Cache.terminal.Print("Loaded settings.config");
    }
    public static void Save()
    {
        InputManager.inputBinds.SaveBinds();

        string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Save");
        string filePath = Path.Combine(directoryPath, "settings.config");

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            Cache.terminal.Print($"Save directory not found, new Save directory created");
        }

        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, JsonConvert.SerializeObject(new Config(), new JsonSerializerSettings { Formatting = Formatting.Indented }));
            dir = filePath;
            Cache.terminal.Print($"settings.config not found, creating new with default settings at: {directoryPath}");
        }
        File.WriteAllText(filePath, JsonConvert.SerializeObject(config, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        dir = filePath;
        Cache.terminal.Print("Saved settings to settings.config");
    }
    public static void ResetAll()
    {
        config = new Config();
        edittingConfig = new Config();

        string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Save");
        string filePath = Path.Combine(directoryPath, "settings.config");

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            Cache.terminal.Print($"Save directory not found, new Save directory created");
        }

        if (!File.Exists(filePath))
        {
            Cache.terminal.Print($"Attempted to delete settings.config but file doesn't exist");
        }
        else
        {
            File.Delete(filePath);
        }

        Save();
        Load();
    }
}
