using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadableFire : MonoBehaviour
{
    public Flammable flammable;

    public Vector2 spreadTimeAB;
    float targetSpreadTime;
    float currentSpreadTime;

    public float spreadDistance;
    private void Start()
    {
        targetSpreadTime = Random.RandomRange(spreadTimeAB.x, spreadTimeAB.y);
    }
    private void Update()
    {
        if (flammable.lit)
        {
            currentSpreadTime += Time.deltaTime;
            if (currentSpreadTime > targetSpreadTime)
            {
                Spread();
                currentSpreadTime = 0f;
                targetSpreadTime = Random.RandomRange(spreadTimeAB.x, spreadTimeAB.y);
            }
        }
    }
    public void Light()
    {
        flammable.Light();
    }
    void Spread()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, spreadDistance,Cache.references.flammableLayerMask);
        foreach (Collider col in colliders){
            SpreadableFire spreadableFire = col.GetComponent<SpreadableFire>();
            spreadableFire.Light();
        }
    }
}
