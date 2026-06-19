using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetRespawnPointClipBrush : ClipBrush
{
    public Transform respawnLocation;
    public bool disableOnTrigger;

    public List<string> scenesToLoadOnRespawn = new List<string>();
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            Cache.health.respawnLocation = respawnLocation.position;
            if (Cache.resetSequenceOnRespawn != null) { Cache.resetSequenceOnRespawn.SaveSequence(); }
            if (Cache.setLoadedInstancesOnRespawn != null) {
                Cache.setLoadedInstancesOnRespawn.SetInstancesToLoadOnRespawn(scenesToLoadOnRespawn);
            }
            gameObject.SetActive(!disableOnTrigger);
        }
    }
}
