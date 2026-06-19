using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSplatterDecalManager : MonoBehaviour
{
    public List<GameObject> smallSplatterDecals = new List<GameObject>();
    public List<GameObject> largeSplatterDecals = new List<GameObject>();

    public float decalMaxDistance;
    public LayerMask decalLayerMask;

    public float smallSplatterRequirement;
    public float largeSplatterRequirement;

    public void Damage(Damage damage)
    {
        if (damage.amount >= smallSplatterRequirement){
            if (damage is DamagePlayer){
                if (damage.type == Totalled.DamageType.Fall){
                    RaycastHit hit;
                    Vector3 direction = Vector3.down;
                    Debug.DrawRay(PlayerTargetInfo.pos, direction * 3f, Color.magenta, 99f);
                    if (Physics.Raycast(PlayerTargetInfo.pos, direction, out hit, decalMaxDistance, decalLayerMask)){
                        if (damage.amount >= largeSplatterRequirement){
                            Instantiate(largeSplatterDecals[Random.RandomRange(0, largeSplatterDecals.Count)], hit.point, Quaternion.LookRotation(hit.normal));
                        }
                        else{
                            Instantiate(smallSplatterDecals[Random.RandomRange(0, smallSplatterDecals.Count)], hit.point, Quaternion.LookRotation(hit.normal));
                        }
                    }
                }
                else{
                    DamagePlayer damagePlayer = (DamagePlayer)damage;
                    RaycastHit hit;
                    Vector3 direction = Vector3.Cross(Vector3.up, damagePlayer.direction).normalized;
                    //Debug.DrawRay(PlayerTargetInfo.pos, direction * 3f, Color.magenta, 99f);
                    if (Physics.Raycast(PlayerTargetInfo.pos, direction, out hit, decalMaxDistance, decalLayerMask)){
                        if (damage.amount >= largeSplatterRequirement){
                            Instantiate(largeSplatterDecals[Random.RandomRange(0, largeSplatterDecals.Count)], hit.point, Quaternion.LookRotation(hit.normal));
                        }
                        else{
                            Instantiate(smallSplatterDecals[Random.RandomRange(0, smallSplatterDecals.Count)], hit.point, Quaternion.LookRotation(hit.normal));
                        }
                    }
                }
            }
        }
    }
}
