using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStunnable
{
    public void Stun(float amount, float imobileTime, float regainSpeedTime);
}
