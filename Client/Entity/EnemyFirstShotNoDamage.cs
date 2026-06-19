using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFirstShotNoDamage : MonoBehaviour
{
    public TurretHealth turretHealth;
    float defaultHp;
    float setHp;
    bool damaged;
    void Start()
    {
        defaultHp = turretHealth.health;
        setHp = 99999f;
        turretHealth.health = setHp;
    }
    private void Update()
    {
        if (!damaged&&turretHealth.health != setHp){
            turretHealth.health = defaultHp;
            damaged = true;
        }
    }
}
