using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoCrate : Pickup
{
    public int ammo;
    public PlayRandomSound sfxOnPickup;
    public PlaySound singleSfxOnPickup;

    public GameObject bulletGib;
    public Vector3 gibSpawnRange;

    bool taken;
    public override void Start()
    {
        base.Start();
        description = ammo.ToString() + "x Ammo";
    }
    public override void Take()
    {
        base.Take();
        if (sfxOnPickup != null) { sfxOnPickup.Play(); sfxOnPickup.transform.SetParent(null); }
        else { singleSfxOnPickup.Play(); singleSfxOnPickup.transform.SetParent(null); }
        taken = true;

        Cache.ammo.ChangeAmmo(ammo);
    }
    private void OnDestroy()
    {
        if (gameObject.scene.isLoaded)
        {
            Vector3 theseGibsSpawnRange = gibSpawnRange * transform.localScale.magnitude;
            if (!taken && bulletGib != null)
            {
                for (int i = 0; i < ammo; i++)
                {
                    Instantiate(bulletGib, transform.position + Utilities.RandomRangeVector3(-theseGibsSpawnRange, theseGibsSpawnRange), Random.rotation);
                }
            }
        }
    }
}
