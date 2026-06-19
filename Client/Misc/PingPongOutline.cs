using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPongOutline : MonoBehaviour
{
    public Outline outliner;
    
    public Color colorA;
    public Color colorB;

    public float speed;
    float t;

    private void Update()
    {
        float t = Mathf.PingPong(Time.time * speed, 1f);
        Color lerpedColor = Color.Lerp(colorA, colorB, t);
        outliner.OutlineColor = lerpedColor;
    }
}
