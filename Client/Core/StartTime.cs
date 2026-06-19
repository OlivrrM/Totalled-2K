using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTime : MonoBehaviour
{
    public static float instanceTime;
    public static float bootupTime;

    public static float instanceFrame;
    public static float bootupFrame;

    public static float buttonPressTime;
    private void Awake()
    {
        instanceTime = 0;
        instanceFrame = 0;

        buttonPressTime = 0;
    }
    private void Update()
    {
        instanceTime += Time.deltaTime;
        bootupTime += Time.deltaTime;

        instanceFrame++;
        bootupFrame++;

        buttonPressTime += Time.deltaTime;
    }
}
