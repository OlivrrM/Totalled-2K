using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CullGroup : MonoBehaviour
{
    public static List<CullGroup> cullGroupsLoaded = new List<CullGroup>();

    public static bool disabled;
    public static int cullGroupIdIndex;
    [HideInInspector] public int id;
    public bool culled;
    public virtual void Start()
    {
        id = cullGroupIdIndex;
        cullGroupIdIndex++;
        gameObject.SetActive(!culled);
        if (disabled) { gameObject.SetActive(true); }
        cullGroupsLoaded.Add(this);
    }
    private void OnDestroy()
    {
        cullGroupsLoaded.Remove(this);
    }
    public virtual void Cull()
    {
        if (!disabled){
            culled = true;
            gameObject.SetActive(false);
        }
    }
    public virtual void Uncull()
    {
        if (!disabled){
            culled = false;
            gameObject.SetActive(true);
        }
    }
}
