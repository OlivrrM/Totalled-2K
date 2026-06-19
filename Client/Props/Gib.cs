using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gib : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public ObjectGibInfo objectGibInfo;
    public GameObject collideParticle;

    public static float lifeTime = 30f;
    public static float cullDistance = 20f;
    float currentLifeTime;

    public bool cullable;
    private void OnValidate()
    {
        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
    }
    private void OnEnable()
    {
        GibManager.activeGibs.Add(this);
    }
    private void OnDisable()
    {
        GibManager.activeGibs.Remove(this);
    }
    private void OnDestroy()
    {
        GibManager.activeGibs.Remove(this);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collideParticle != null){
            if (collision.gameObject.layer != 9){
                Instantiate(collideParticle, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
    public void UpdateLifetime(Vector3 playerPos, float deltaTime)
    {
        currentLifeTime += deltaTime;
        if (currentLifeTime > lifeTime){
            if (!meshRenderer.isVisible &&
                (playerPos - transform.position).sqrMagnitude > cullDistance * cullDistance){
                Destroy(gameObject);
            }
        }
    }
}
