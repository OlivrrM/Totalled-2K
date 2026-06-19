using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public class InvokeTest : MonoBehaviour
{
    public void TestFunction(float num,string str)
    {
        print("worked! "+num+ " "+str);
    }
}