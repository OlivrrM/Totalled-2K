using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSensitivityManager : ValueMultiplierManager
{
    Vector2 defaultSensitivity;
    private void Awake()
    {
        Cache.mouseSensitivityManager = this;
    }
    private void Start()
    {
        defaultSensitivity = new Vector2(InputManager.mouseSensX, InputManager.mouseSensY);
    }
    private void Update()
    {
        InputManager.mouseSensX = defaultSensitivity.x;
        InputManager.mouseSensY = defaultSensitivity.y;
        foreach (KeyValuePair<string, object> value in values)
        {
            InputManager.mouseSensX *= ((Vector2)value.Value).x;
            InputManager.mouseSensY *= ((Vector2)value.Value).y;
        }
    }
}
