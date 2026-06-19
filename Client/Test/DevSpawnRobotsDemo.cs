using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevSpawnRobotsDemo : MonoBehaviour
{
    public float minRadius;

    public Vector2 radius;
    public GameObject robot;
    public int robotAmount;

    public GameObject healthKit;
    public int healthKitAmount;

    public GameObject grenade;
    public int grenadeAmount;

    public GameObject ammo;
    public int ammoAmount;

    public GameObject propaneTank;
    public int propaneTankAmount;
    private void Start()
    {
        if (Application.isEditor) { NewWave(); }
    }
    public void NewWave()
    {
        for (int i = 0; i < robotAmount; i++)
        {
            Instantiate(robot, SpawnPos(), Quaternion.identity);
        }
        for (int i = 0; i < healthKitAmount; i++)
        {
            Instantiate(healthKit, SpawnPos(), Quaternion.identity);
        }
        for (int i = 0; i < grenadeAmount; i++)
        {
            Instantiate(grenade, SpawnPos(), Quaternion.identity);
        }
        for (int i = 0; i < ammoAmount; i++)
        {
            Instantiate(ammo, SpawnPos(), Quaternion.identity);
        }
        for (int i = 0; i < propaneTankAmount; i++)
        {
            Instantiate(propaneTank, SpawnPos(), Quaternion.identity);
        }
    }
    Vector3 SpawnPos()
    {
        Vector3 spawnPos = new Vector3(Random.RandomRange(-radius.x, radius.x),1.5f, Random.RandomRange(-radius.y, radius.y));
        while (true)
        {
            if (spawnPos.x < minRadius && spawnPos.x > -minRadius)
            {
                spawnPos = new Vector3(Random.RandomRange(-radius.x, radius.x),1.5f, Random.RandomRange(-radius.y, radius.y));
            }
            else if (spawnPos.z < minRadius && spawnPos.z > -minRadius)
            {
                spawnPos = new Vector3(Random.RandomRange(-radius.x, radius.x),1.5f, Random.RandomRange(-radius.y, radius.y));
            }
            else { return spawnPos; }
        }
    }
}
