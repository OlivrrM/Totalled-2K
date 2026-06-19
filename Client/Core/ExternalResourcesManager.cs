using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExternalResourcesManager : MonoBehaviour
{
    //List of loaded assetbundles
    public static Dictionary<string, AssetBundle> assetBundles = new Dictionary<string, AssetBundle>();

    //Function for loading desired assetbundle, then adding to loaded assetbundle list
    public static string LoadAssetBundle(string assetBundleName,string assetBundlePath)
    {
        AssetBundle assetBundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(Application.streamingAssetsPath, assetBundlePath));
        if (assetBundle != null){
            assetBundles.Add(assetBundleName, assetBundle);
            //print(assetBundles[assetBundleName] == null);
            return $"Loaded asset {assetBundleName}";
        }
        else { return $"Could not find asset bundle '{assetBundleName}' within StreamingAssets folder"; }
    }

    //Function for removing loaded assetbundle from list, or clearing all
    public static string UnloadAssetBundle(string assetBundleName)
    {
        if (assetBundles.ContainsKey(assetBundleName))
        {
            assetBundles[assetBundleName].Unload(true);
            assetBundles.Remove(assetBundleName);
            return $"Unloaded asset {assetBundleName}";
        }
        else
        {
            return $"Unknown assetbundle {assetBundleName}";
        }
    }
    
    //Then create a class for added prefabs from assetbundles into a list within resources called something like "Externally loaded prefabs"
    public static string LoadPrefabAsset(string assetBundleName, string prefabName)
    {
        if (assetBundles.ContainsKey(assetBundleName))
        {
            GameObject go = assetBundles[assetBundleName].LoadAsset<GameObject>(prefabName);
            if (go != null)
            {
                Cache.references.externallyLoadedPrefabs.Add(prefabName, go);
                return $"Loaded prefab {go.name}";
            }
            else { return $"Unknown prefab '{prefabName}' within assetbundle '{assetBundleName}'"; }
        }
        else
        {
            return $"Unknown assetbundle {assetBundleName}";
        }
    }

    public static string UnloadPrefabAsset(string prefabName)
    {
        if (Cache.references.externallyLoadedPrefabs.ContainsKey(prefabName))
        {
            Cache.references.externallyLoadedPrefabs.Remove(prefabName);
            return $"Loaded prefab {Cache.references.externallyLoadedPrefabs[prefabName]}";
        }
        else
        {
            return $"Unknown prefab {prefabName}";
        }
    }

    //The instantiate command can then use "externally loaded prefabs" from resources
}
