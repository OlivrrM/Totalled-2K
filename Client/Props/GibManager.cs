using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GibManager : MonoBehaviour
{
    public static List<Gib> activeGibs = new List<Gib>();

    private const float cullUpdateInterval = 0.1f; // 10x/sec
    private float timeAccumulator = 0f;

    public static void Clear() // This doesnt work cus Gib removes itself from list when destroyed/disabled. A duplicate list will need to be made, then the main list cleared after.
    {
        List<Gib> gibsToRemove = new List<Gib>();
        for (int i = 0; i < activeGibs.Count; i++)
        {
            gibsToRemove.Add(activeGibs[i]);
        }
        for (int i = 0; i < gibsToRemove.Count; i++){
            if (gibsToRemove[i] != null) { Destroy(gibsToRemove[i].gameObject); }
        }
        activeGibs.Clear();
    }

    void FixedUpdate()
    {
        timeAccumulator += Time.fixedDeltaTime;
        if (timeAccumulator >= cullUpdateInterval){
            timeAccumulator = 0f;
            Vector3 playerPos = Cache.surfCharacter.transform.position;
            for (int i = activeGibs.Count - 1; i >= 0; i--){
                activeGibs[i].UpdateLifetime(playerPos, cullUpdateInterval);
            }
        }
    }
}
