using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClipBrushes : MonoBehaviour
{
    public static bool active;

    public static List<ClipBrush> activeClipBrushes = new List<ClipBrush>();
    public static void Toggle()
    {
        active = !active;
        if (active) { Show(); }
        else { Hide(); }
    }
    public static void Show()
    {
        for (int i = 0; i < activeClipBrushes.Count; i++){
            activeClipBrushes[i].Show();
        }
    }
    public static void Hide()
    {
        for (int i = 0; i < activeClipBrushes.Count; i++){
            activeClipBrushes[i].Hide();
        }
    }
}
