using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpeedCounter : MonoBehaviour
{
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI speedIncreaseText;

    float previousJumpSpeed;
    float currentJumpSpeed;
    float speedDifference;

    public Color speedIncreaseColor;
    public Color speedDecreaseColor;
    public Color speedStagnantColor;
    private void Awake()
    {
        Cache.speedCounter = this;
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        speedText.gameObject.SetActive(true);
        speedIncreaseText.gameObject.SetActive(true);
    }
    private void OnDisable()
    {
        speedText.gameObject.SetActive(false);
        speedIncreaseText.gameObject.SetActive(false);
    }
    private void Update()
    {
        speedText.text = GetSpeed().ToString();
        if (FragMovementListener.justJumped) 
        {
            currentJumpSpeed = GetSpeed();
            speedDifference = System.MathF.Round(currentJumpSpeed - previousJumpSpeed,2);
            previousJumpSpeed = currentJumpSpeed;
        }
        if (speedDifference > 0) { speedIncreaseText.color = speedIncreaseColor; }
        else if (speedDifference < 0) { speedIncreaseText.color = speedDecreaseColor; }
        else { speedIncreaseText.color = speedStagnantColor; }
        speedIncreaseText.text = (speedDifference > 0?"+":"")+ speedDifference.ToString();
    }
    float GetSpeed()
    {
        return System.MathF.Round(new Vector2(Cache.moveData.Velocity.x, Cache.moveData.Velocity.z).magnitude,2);
    }
}
