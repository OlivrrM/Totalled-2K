using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCanvas : MonoBehaviour
{
    private void Awake()
    {
        Cache.dynamicCanvas = gameObject.GetComponent<RectTransform>();
    }
}
