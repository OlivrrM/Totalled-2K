using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallKick : MonoBehaviour ///UNUSED FOR NOW
{
    /*
    public LayerMask layerMask;
    public float wallKickDistance;
    public float wallKickPower;

    public KeyCode shiftKey;
    float shiftKeyTime;
    public float shiftKeyDecaySpeed;
    private void Update()
    {
        shiftKeyTime -= Time.deltaTime * shiftKeyDecaySpeed;
        if (Input.GetKeyDown(shiftKey)){
            shiftKeyTime = 1f;
        }
        if (shiftKeyTime > 0f && Input.GetKeyDown(Cache.surfCharacter.JumpButton)){
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, wallKickDistance, layerMask)){
                if (shiftKeyTime >= 0.8f) { Cache.moveData.Velocity += Cache.surfCharacter.transform.forward * Cache.moveData.Velocity.magnitude * wallKickPower; }
                else { Cache.moveData.Velocity += Cache.surfCharacter.transform.forward * Cache.moveData.Velocity.magnitude * (1 + ((wallKickPower - 1) * shiftKeyTime)); }
            }
        }
    }
    */
}
