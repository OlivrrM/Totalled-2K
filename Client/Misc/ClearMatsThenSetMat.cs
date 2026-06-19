using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearMatsThenSetMat : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public void ClearThenSetMat(Material material)
    {
        meshRenderer.materials = new Material[1];
        meshRenderer.material = material;
    }
}
