using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class RunCommandOnLevel : MonoBehaviour
{
    public Terminal terminal;
    public Dictionary<string, string> commands = new Dictionary<string, string>();

    int executeFrames;
    void Awake()
    {
        Cache.runCommandOnLevel = this;
        SceneManager.sceneLoaded += OnSceneWasLoaded;
    }
    void OnSceneWasLoaded(Scene scene, LoadSceneMode loadMode)
    {
        executeFrames = 10;
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneWasLoaded;
    }
    void Start()
    {
        LoadCommands();
    }
    public void LoadCommands()
    {
        commands.Clear();

        string subfolderPath = Path.Combine(Application.dataPath, "..", "Run");

        if (Directory.Exists(subfolderPath))
        {
            string[] files = Directory.GetFiles(subfolderPath, "*.t2kc");

            foreach (string filePath in files)
            {
                if (File.Exists(filePath))
                {
                    bool passed = true;
                    string fileContents = File.ReadAllText(filePath);
                    fileContents = fileContents.Replace("\r\n", "").Replace("\r", "");
                    List<CommandOnInstance> commandsOnInstances = new List<CommandOnInstance>();
                    try { commandsOnInstances = JsonConvert.DeserializeObject<List<CommandOnInstance>>(fileContents); }
                    catch (System.Exception e) { Debug.LogError($"Invalid JSON formatting within {filePath}: {e.Message}"); passed = false; }
                    if (passed)
                    {
                        for (int i = 0; i < commandsOnInstances.Count; i++)
                        {
                            if (commands.ContainsKey(commandsOnInstances[i].instance))
                            {
                                commands[commandsOnInstances[i].instance] += $";{commandsOnInstances[i].command}";
                            }
                            else { commands.Add(commandsOnInstances[i].instance, commandsOnInstances[i].command); }
                        }
                    }
                }
                else
                {
                    Debug.LogError("File not found: " + filePath);
                }
            }
        }
        Cache.terminal.Print("Loaded commands from disk");
    }
    private void Update()
    {
        executeFrames--;
        if (executeFrames == 0)
        {
            string instanceName = SceneManager.GetActiveScene().name;
            foreach (KeyValuePair<string, string> cmd in commands)
            {
                if (cmd.Key == instanceName)
                {
                    terminal.PreExecute(cmd.Value);
                }
            }
        }
    }
}
