using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetNameToParentOnStart : MonoBehaviour
{
    public string insert;
    private void Start()
    {
        transform.name = transform.parent.name + insert;
    }
}
