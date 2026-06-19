using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAOE : MonoBehaviour
{
    public Vector2 aoeLifetimeAB;

    public ParticleSystem[] particleSystems;
    public ParticleSystem floorMist;
    public Collider collider;
    public SmoothDisableSfx fireIdleSfx;

    private void Start()
    {
        Invoke("Decay", Random.RandomRange(aoeLifetimeAB.x, aoeLifetimeAB.y));   
    }
    void Decay()
    {
        for (int i = 0; i < particleSystems.Length; i++){
            particleSystems[i].Stop();
        }
        floorMist.Pause();
        collider.enabled = false;
        fireIdleSfx.Disable(1f);
    }
}
