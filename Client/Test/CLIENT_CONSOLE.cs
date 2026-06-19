using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public class CLIENT_CONSOLE : MonoBehaviour
{
    public InvokeTest invokeTestScript;
    public Type type;
    private void Start()
    {
        type = Type.GetType("InvokeTest");

        MethodInfo typeMethod = type.GetMethod("TestFunction");
        object obj = typeMethod.Invoke(invokeTestScript,new object[] { 100f,"Yay!" });
    }
}
