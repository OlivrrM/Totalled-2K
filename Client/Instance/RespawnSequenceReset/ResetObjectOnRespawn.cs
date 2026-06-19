using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetObjectOnRespawn : MonoBehaviour
{
    public bool backup;
    public bool flagForReset;
    private void Start()
    {
        if (Cache.resetSequenceOnRespawn.forgottenObjectPositions.Contains(transform.localPosition)) { Destroy(gameObject); }
        if (!backup){
            if (Cache.resetSequenceOnRespawn != null){
                Cache.resetSequenceOnRespawn.RememberGameObject(gameObject);
            }

        }
    }
}
