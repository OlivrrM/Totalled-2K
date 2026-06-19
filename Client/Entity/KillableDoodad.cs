using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillableDoodad : MonoBehaviour
{
    public virtual void Die()
    {
        Destroy(gameObject);
    }
}
