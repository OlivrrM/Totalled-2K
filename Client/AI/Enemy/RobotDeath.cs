using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotDeath : MonoBehaviour
{
    public virtual void Die(Damage damage)
    {
        Destroy(gameObject);
    }
}
