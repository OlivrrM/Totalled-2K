using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snooz : MonoBehaviour
{
    public void Sleep(int milliseconds)
    {
        System.Threading.Thread.Sleep(milliseconds);
    }
}
