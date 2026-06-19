using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootDropOnDeath : MonoBehaviour
{
    public List<GameObject> lootTable_Items = new List<GameObject>();
    public List<float> lootTable_Chances = new List<float>();

    public float lootDropForce;
    public float lootDropTorque;
    private void OnDestroy()
    {
        if (gameObject.scene.isLoaded){
            for (int i = 0; i < lootTable_Items.Count; i++){
                if (Random.RandomRange(0, 100) < lootTable_Chances[i]){
                    GameObject loot = Instantiate(lootTable_Items[i], transform.position, Random.rotation);
                    Rigidbody rb = loot.GetComponent<Rigidbody>();
                    rb.AddForce(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * lootDropForce);
                    rb.AddTorque(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * lootDropTorque);
                }
            }
        }
    }
}
