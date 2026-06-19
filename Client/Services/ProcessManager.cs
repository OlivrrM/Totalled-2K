using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class ProcessManager : MonoBehaviour
{
    public void Calc()
    {
        Process.Start("Calc.exe");
    }
}
