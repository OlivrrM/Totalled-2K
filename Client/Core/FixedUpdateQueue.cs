using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FixedUpdateQueue : MonoBehaviour
{
    public static List<Function> functions = new List<Function>(); ///Function name, class name
    private void Awake()
    {
        functions.Clear();
    }
    private void Start()
    {
        functions = functions.OrderBy(func => func.orderIndex).ToList();
    }
    private void Update()
    {
        for (int i = 0; i < functions.Count; i++){
            try
            {
                functions[i].instance.Invoke(functions[i].function, 0f);
            }
            catch (System.Exception e) { print("Error invoking function in FixedUpdateQueue: " + e.Message); }
        }
    }
}
