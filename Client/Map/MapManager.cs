#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
//using System.Windows.Forms;
using SFB;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine.Serialization;

[Serializable]
public class SerializedComponent
{
    public string TypeName;
    public string Data;
}

[Serializable]
public class SerializedGameObject
{
    public string Name;
    public string Tag;
    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Scale;
    public List<SerializedComponent> Components = new List<SerializedComponent>();
}

[Serializable]
public class SerializedMap
{
    public string MapName;
    public List<SerializedGameObject> GameObjects = new List<SerializedGameObject>();
}

public class MapManager : MonoBehaviour
{
    public static JsonSerializerSettings serializationSettings = new JsonSerializerSettings
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        Formatting = Formatting.Indented
    };
    public static GameObject thisGO;
    private void Start()
    {
        
    }
    public static void SerializeMap(string mapName, out string exportLocation)
    {
        var extensionList = new[] {
            new ExtensionFilter("Totalled 2k Map", "t2km")
        };
        var path = StandaloneFileBrowser.SaveFilePanel("Save Map", "", mapName, extensionList);

        
        SerializedMap serializedMap = new SerializedMap
        {
            MapName = mapName
        };
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject rootObj in rootObjects)
        {
            SerializeGameObject(rootObj, serializedMap);
        }
        string json = JsonConvert.SerializeObject(serializedMap);
        File.WriteAllText(path, json);
        exportLocation = path;
    }

    private static void SerializeGameObject(GameObject gameObject, SerializedMap serializedScene)
    {
        try
        {
            if (gameObject != Cache.surfCharacter && gameObject != Cache.gameScripts)
            {
                SerializedGameObject serializedObject = new SerializedGameObject
                {
                    Name = gameObject.name,
                    Tag = gameObject.tag,
                    Position = gameObject.transform.position,
                    Rotation = gameObject.transform.eulerAngles,
                    Scale = gameObject.transform.localScale
                };
                foreach (Component component in gameObject.GetComponents<Component>())
                {
                    SerializedComponent serializedComponent = new SerializedComponent
                    {
                        TypeName = component.GetType().ToString(),
                        Data = JsonConvert.SerializeObject(component, serializationSettings)
                    };
                    serializedObject.Components.Add(serializedComponent);
                }
                serializedScene.GameObjects.Add(serializedObject);
                foreach (Transform child in gameObject.transform)
                {
                    SerializeGameObject(child.gameObject, serializedScene);
                }
            }
        }
        catch(Exception e)
        {
            Debug.LogError($"Error serializing {gameObject.name}: {e.Message}");
        }
    }
}