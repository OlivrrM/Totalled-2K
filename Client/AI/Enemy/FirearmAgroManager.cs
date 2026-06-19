using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirearmAgroManager : MonoBehaviour
{
    public static float multiplier;
    public float decaySpeed;
    public static void Shot(float amount)
    {
        multiplier += amount;
    }
    private void Update()
    {
        multiplier -= Time.deltaTime * decaySpeed;
        multiplier = Mathf.Clamp(multiplier, 1f, 1.6f);
    }
}
