using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ClipBrush : MonoBehaviour
{
    MeshRenderer meshRenderer;
    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        ClipBrushes.activeClipBrushes.Add(this);
        if (!ClipBrushes.active) { Hide(); }
    }
    public void Show(){
        meshRenderer.enabled = true;
    }
    public void Hide(){
        meshRenderer.enabled = false;
    }
    private void OnDestroy(){
        ClipBrushes.activeClipBrushes.Remove(this);
    }
}
