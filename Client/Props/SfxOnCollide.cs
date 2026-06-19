using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxOnCollide : MonoBehaviour
{
    public PlaySound sfx;
    public PlayRandomSound rsfx;
    public Vector2 pitchClamp;
    public Vector2 velocityRange;
    private void OnCollisionEnter(Collision collision)
    {
        if (sfx != null) { sfx.transform.position = collision.contacts[0].point; }
        else if (rsfx != null) { rsfx.transform.position = collision.contacts[0].point; }
        float pitch = Mathf.Lerp(pitchClamp.x, pitchClamp.y, 1 - Mathf.InverseLerp(velocityRange.x, velocityRange.y, collision.relativeVelocity.magnitude));
        if (sfx != null) { sfx.Play(true, pitch); }
        else if (rsfx != null) { rsfx.Play(pitch, pitch); }
    }
}
