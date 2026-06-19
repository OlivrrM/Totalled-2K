using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class References : MonoBehaviour
{
    public Dictionary<string, GameObject> items = new Dictionary<string, GameObject>();
    public List<string> itemsKEYS;
    public List<GameObject> itemsVALUES;

    public Dictionary<string, GameObject> bulletHoles = new Dictionary<string, GameObject>();
    public List<string> bulletHolesKEYS = new List<string>();
    public List<GameObject> bulletHolesVALUES = new List<GameObject>();

    public Dictionary<string, GameObject> meleeImpacts = new Dictionary<string, GameObject>();
    public List<string> meleeImpactsKEYS = new List<string>();
    public List<GameObject> meleeImpactsVALUES = new List<GameObject>();

    public Dictionary<string, GameObject> spawnables = new Dictionary<string, GameObject>();
    public List<string> spawnablesKEYS = new List<string>();
    public List<GameObject> spawnablesVALUES = new List<GameObject>();

    public Dictionary<string, GameObject> doodads = new Dictionary<string, GameObject>();
    public List<string> doodadsKEYS = new List<string>();
    public List<GameObject> doodadsVALUES = new List<GameObject>();

    public Dictionary<string, RenderFeatureManager> renderFeatures = new Dictionary<string, RenderFeatureManager>();
    public List<string> renderFeaturesKEYS = new List<string>();
    public List<RenderFeatureManager> renderFeaturesVALUES = new List<RenderFeatureManager>();

    public Dictionary<string, List<Keybind>> binds = new Dictionary<string, List<Keybind>>();

    public Dictionary<string, GameObject> externallyLoadedPrefabs = new Dictionary<string, GameObject>();

    public Dictionary<string, GameObject> gamesystems = new Dictionary<string, GameObject>();
    public List<string> gamesystemsKEYS = new List<string>();
    public List<GameObject> gamesystemsVALUES = new List<GameObject>();

    public LayerMask playerLayerMask;
    public LayerMask solidLayerMask;
    public LayerMask bulletLayerMask;
    public LayerMask enemyBulletLayerMask;
    public LayerMask flammableLayerMask;
    public LayerMask enemyLayerMask;
    public LayerMask breakableForceIncludeLayerMask;
    private void Awake()
    {
        Object.DontDestroyOnLoad(this);

        for (int i = 0; i < itemsKEYS.Count; i++){
            items.Add(itemsKEYS[i], itemsVALUES[i]);
        }

        for (int i = 0; i < bulletHolesKEYS.Count; i++){
            bulletHoles.Add(bulletHolesKEYS[i], bulletHolesVALUES[i]);
        }

        for (int i = 0; i < meleeImpactsKEYS.Count; i++){
            meleeImpacts.Add(meleeImpactsKEYS[i], meleeImpactsVALUES[i]);
        }

        for (int i = 0; i < spawnablesKEYS.Count; i++){
            spawnables.Add(spawnablesKEYS[i], spawnablesVALUES[i]);
        }

        for (int i = 0; i < doodadsKEYS.Count; i++){
            doodads.Add(doodadsKEYS[i], doodadsVALUES[i]);
        }

        for (int i = 0; i < renderFeaturesKEYS.Count; i++){
            renderFeatures.Add(renderFeaturesKEYS[i], renderFeaturesVALUES[i]);
        }

        for (int i = 0; i < gamesystemsKEYS.Count; i++){
            gamesystems.Add(gamesystemsKEYS[i], gamesystemsVALUES[i]);
        }

        binds = InputManager.inputBinds.savedBinds.binds;
    }
}
