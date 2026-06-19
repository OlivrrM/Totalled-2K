using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotScreenFaceManager : MonoBehaviour
{
    public Robot robot;

    public MeshRenderer stunnedScreenMeshRenderer;
    Material stunScreenMaterialInstance;
    public SpriteRenderer stunScreenBloom;

    private void Start()
    {
        stunScreenMaterialInstance = new Material(stunnedScreenMeshRenderer.material);
        stunnedScreenMeshRenderer.material = stunScreenMaterialInstance;
    }

    private void Update()
    {
        if (!robot.stunRegaining)
        {
            stunScreenMaterialInstance.color = Color.white;
            stunScreenBloom.color = Color.white;
        }
        else
        {
            Color targetColor = Utilities.Invisible(Color.white);
            stunScreenMaterialInstance.color = Color.Lerp(stunScreenMaterialInstance.color, targetColor, Time.deltaTime * 2f);
            stunScreenBloom.color = Color.Lerp(stunScreenBloom.color, targetColor, Time.deltaTime * 2f);
        }
    }
}
