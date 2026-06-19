using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstAidHighlightsManager : MonoBehaviour
{
    public FirstAid firstAid;
    public MeshRenderer firstAidMeshRenderer;
    Material highlightMat;
    public EmissionIntensityPingPong emissionIntensityPingPong;

    bool disabled = false;
    private void Start()
    {
        highlightMat = firstAidMeshRenderer.materials[1];
    }
    public void EnableHighlights()
    {
        emissionIntensityPingPong.emissionDisabled = false;
        firstAidMeshRenderer.SetMaterials(new List<Material> { firstAidMeshRenderer.material, highlightMat });
    }
    public void DisableHighlights()
    {
        emissionIntensityPingPong.emissionDisabled = true;
        firstAidMeshRenderer.SetMaterials(new List<Material> { firstAidMeshRenderer.material, null });
    }
    private void Update()
    {
        if (disabled){
            if (firstAid.health > 0f){
                EnableHighlights();
                disabled = false;
            }
        }
        else{
            if (firstAid.health <=0f){
                DisableHighlights();
                disabled = true;
            }
        }
    }
}
