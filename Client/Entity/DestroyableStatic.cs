using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SubsystemsImplementation;

public class DestroyableStatic : MonoBehaviour, IDamageable
{
    public float health;
    bool dead;
    public MeshFilter[] meshFilters;
    public Mesh[] meshes;
    public GameObject fx;
    public Transform fxPos;
    public PlaySound sfx;

    public Collider colliderToDisable;
    public virtual void Damage(Damage damage)
    {
        health -= damage.amount;
        if (health <= 0 && !dead) {
            dead = true;
            if (fx != null) { Instantiate(fx, fxPos?.position ?? transform.position, fxPos?.rotation ?? Quaternion.identity); }
            if (sfx != null) { sfx.Play(); }
            if (meshFilters.Length > 0) {
                for (int i = 0; i < meshFilters.Length; i++){
                    meshFilters[i].mesh = meshes[i];
                }
            }
            if (colliderToDisable != null) { colliderToDisable.enabled = false; }
        }
    }
}
