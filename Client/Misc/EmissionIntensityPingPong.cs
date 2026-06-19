using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class EmissionIntensityPingPong : MonoBehaviour
{
    public Material mat;
    public float speed;
    Color lerpedColor;
    Color baseEmissionColor;

    [HideInInspector] public bool emissionDisabled;

    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
        baseEmissionColor = mat.GetColor("_EmissionColor");
    }

    private void Update()
    {
        float intensity = Mathf.PingPong(Time.time * speed, 1f);
        lerpedColor = Color.Lerp(emissionDisabled?Color.black:baseEmissionColor, Color.black, intensity);
        mat.SetColor("_EmissionColor", lerpedColor);
    }
}