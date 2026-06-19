using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlankBlockedDoor : MonoBehaviour
{
    public PlankBarrier[] plankBarriers;
    public Door door;
    private void Update()
    {
        if (door.locked){
            int planksBroken = 0;
            for (int i = 0; i < plankBarriers.Length; i++){
                if (plankBarriers[i].broken){
                    planksBroken++;
                }
            }
            if (planksBroken >= plankBarriers.Length)
            {
                door.Unlock();
            }
        }
    }
}
