using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderFeatureManager : MonoBehaviour
{
    public bool enabled;
    public virtual void Enable()
    {
        enabled = true;
    }
    public virtual void Disable()
    {
        enabled = false;
    }
    public void Toggle()
    {
        if (enabled) { Disable(); }
        else { Enable(); }
    }
}
