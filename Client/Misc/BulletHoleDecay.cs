using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BulletHoleDecay : MonoBehaviour
{
    public float lifetime;
    public DecalProjector projector;

    float opacity;
    private void Start()
    {
        opacity = projector.fadeFactor;
        Destroy(gameObject, lifetime);
    }
    private void Update()
    {
        lifetime -= Time.deltaTime;
        projector.fadeFactor = Mathf.Lerp(0f, opacity, lifetime);
    }
}
