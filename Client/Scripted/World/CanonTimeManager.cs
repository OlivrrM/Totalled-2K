using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonTimeManager : MonoBehaviour
{
    public DateTime gameTime;
    public float secondsPerRealSecond = 1f;

    void Start()
    {
        Cache.canonTimeManager = this;
        gameTime = new DateTime(1999, 12, 31, 23, 58, 0);
    }

    void Update()
    {
        gameTime = gameTime.AddSeconds(Time.deltaTime * secondsPerRealSecond);
    }
    public string GetTimeAsString()
    {
        return gameTime.ToString("HH:mm:ss");
    }
}
