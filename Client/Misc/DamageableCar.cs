using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableCar : MonoBehaviour, IDamageable
{
    public ObjectJumpBob objectJumpBob;
    CarAlarmTrigger carAlarmTrigger;
    private void Start()
    {
        carAlarmTrigger = gameObject.GetComponent<CarAlarmTrigger>();
    }
    public void Damage(Damage damage)
    {
        if (damage.type == Totalled.DamageType.Explosion){
            objectJumpBob.Bounce(damage.amount * objectJumpBob.hitVelocityRange.y,Vector3.zero);
            if (carAlarmTrigger != null) { carAlarmTrigger.Trigger(); }
        }
    }
}
