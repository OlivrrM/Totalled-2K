using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Totalled;

public class RobotHealth : MonoBehaviour
{
    public Robot robot;

    public float maxHealth;
    public float health;

    public RobotDeath robotDeath;
    bool dead;

    public Dictionary<DamageType, float> damageTypeMultipliers = new Dictionary<DamageType, float>();
    public List<DamageType> damageTypeMultipliers_KEYS = new List<DamageType>();
    public List<float> damageTypeMultipliers_VALUES = new List<float>();
    private void Start()
    {
        health = maxHealth;

        for (int i = 0; i < damageTypeMultipliers_KEYS.Count; i++){
            damageTypeMultipliers.Add(damageTypeMultipliers_KEYS[i], damageTypeMultipliers_VALUES[i]);
        }
    }
    public void Damage(Damage damage, bool critical = false)
    {
        foreach (KeyValuePair<DamageType,float> multiplier in damageTypeMultipliers){
            if (multiplier.Key == damage.type){
                damage.amount *= multiplier.Value;
            }
        }

        if (damage.attacker == Cache.surfCharacter.gameObject) { Cache.hitMarkerManager.Hit(damage.amount, critical); }
        health -= damage.amount;
        robot.damagedAgroMultiplier += robot.damageAgroMultiplierIncreaseAmount;
        if (health <= 0f)
        {
            Die(damage);
        }
    }
    void Die(Damage damage)
    {
        if (!dead){
            robotDeath.Die(damage);
            dead = true;
        }
    }
}
