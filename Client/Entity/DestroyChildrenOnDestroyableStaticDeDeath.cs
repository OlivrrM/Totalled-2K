using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DestroyChildrenOnDestroyableStaticDeDeath : DestroyableStatic
{
    public GameObject[] childrenToDestroy;
    public override void Damage(Damage damage)
    {
        base.Damage(damage);
        if (health <= 0f){
            for (int i = 0; i < childrenToDestroy.Length; i++){
                Destroy(childrenToDestroy[i]);
            }
        }
    }
}
