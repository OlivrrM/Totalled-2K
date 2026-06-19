using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCharacterRenderManager : MonoBehaviour
{
    public MeshRenderer[] meshRenderers;
    public SpriteRenderer[] spriteRenderers;
    private void Start()
    {
        Cache.thirdPersonCharacterRenderManager = this;
    }
    public void SetActive(bool render)
    {
        for (int i = 0; i < meshRenderers.Length; i++){
            meshRenderers[i].enabled = render;
        }
        for (int i = 0; i < spriteRenderers.Length; i++){
            spriteRenderers[i].enabled = render;
        }
    }
}
