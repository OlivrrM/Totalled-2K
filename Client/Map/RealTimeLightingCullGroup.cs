using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealTimeLightingCullGroup : CullGroup
{
    public List<Light> lights;
    public override void Start()
    {
        base.Start();
        ActivateLights(!culled);
    }
    public override void Cull()
    {
        base.Cull();
        ActivateLights(false);
    }
    public override void Uncull()
    {
        base.Uncull();
        ActivateLights(true);
    }
    public void ActivateLights(bool enable)
    {
        for (int i = 0; i < lights.Count; i++){
            lights[i].enabled = enable;
        }
    }
}
