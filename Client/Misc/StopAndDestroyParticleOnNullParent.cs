using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopAndDestroyParticleOnNullParent : MonoBehaviour
{
    ///Work around because of this bug:
    //https://discussions.unity.com/t/particle-systems-particles-destroyed-when-changing-parent/642449/4

    public ParticleSystem particle;
    public float lifetime;
    private void Update()
    {
        if (transform.parent == null)
        {
            Stop();
        }
    }
    void Stop()
    {
        particle.Stop();
        Destroy(gameObject, lifetime);
    }
}
