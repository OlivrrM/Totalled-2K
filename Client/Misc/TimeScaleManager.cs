using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleManager : MonoBehaviour
{
    public static Dictionary<string, object> values = new Dictionary<string, object>();
    public static void AddValue(string name, object value)
    {
        if (values.ContainsKey(name))
        {
            Debug.LogError("Attempted to add new value with name '" + name + "', however value already existed.");
        }
        else { values.Add(name, value); }
    }
    public static void UpdateValue(string targetName, object value)
    {
        values[targetName] = value;
    }
    private void Update()
    {
        Time.timeScale = 1f;
        foreach (KeyValuePair< string, object> scale in values){
            Time.timeScale *= System.Convert.ToSingle(scale.Value);
        }
    }
}
