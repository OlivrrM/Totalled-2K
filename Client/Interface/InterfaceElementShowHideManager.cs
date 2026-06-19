using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceElementShowHideManager : MonoBehaviour
{
    public bool hidden;
    public RectTransform rect;
    public Vector2 hiddenPos;
    Vector2 unhiddenPos;
    public float speed;
    private void Start()
    {
        unhiddenPos = rect.anchoredPosition;
    }
    private void Update()
    {
        rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, hidden ? hiddenPos : unhiddenPos, Time.unscaledDeltaTime * speed);
    }
}
