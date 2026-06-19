using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunBob : MonoBehaviour
{
    public CameraRotationOffsetManager cameraRotationOffsetManager;
    public AnimationCurve y;
    public AnimationCurve x;

    float yPointer;
    public float yBobSpeed;
    public float yBobAmount;

    float xPointer;
    public float xBobSpeed;
    public float xBobAmount;

    float runningMultiplier;

    float time;

    public float stopRunningSmoothness;

    public Dictionary<string, bool> enablers = new Dictionary<string, bool>();
    public Dictionary<string, float> speedMultipliers = new Dictionary<string, float>();
    public Dictionary<string, float> amountMultipliers = new Dictionary<string, float>();
    private void Start()
    {
        cameraRotationOffsetManager.AddOffset("RunBob", new CameraRotationOffset { offset = new Vector4(0, 0, 0, 0) });
        enablers.Add("Running", false);
        Cache.runBob = this;
    }
    private void Update()
    {
        float speedMultiplier = 0f;
        if (speedMultipliers.Count > 0){
            foreach (KeyValuePair<string, float> mltp in speedMultipliers){
                speedMultiplier += mltp.Value;
            }
        }
        else { speedMultiplier = 1f; }
        time += Time.deltaTime * runningMultiplier * FragMovementListener.hSpeedPercentage * speedMultiplier;
        enablers["Running"] = FragMovementListener.running;
        bool enabled = false;
        foreach (KeyValuePair<string,bool> enabler in enablers){
            if (enabler.Value) { enabled = true; break; }
        }
        if (enabled) { runningMultiplier = 1; }
        else { runningMultiplier = Mathf.Lerp(runningMultiplier, Utilities.BoolToInt(enabled), Time.deltaTime * stopRunningSmoothness); }
        yPointer = Mathf.PingPong(time * yBobSpeed, 1);
        xPointer = Mathf.PingPong(time * xBobSpeed, 1);
        float amountMultiplier = 0f;
        if (amountMultipliers.Count > 0)
        {
            foreach (KeyValuePair<string, float> mltp in amountMultipliers)
            {
                amountMultiplier += mltp.Value;
            }
        }
        else { amountMultiplier = 1f; }
        cameraRotationOffsetManager.UpdateOffset("RunBob", new CameraRotationOffset { offset = new Vector4(y.Evaluate(yPointer) * (yBobAmount * amountMultiplier), x.Evaluate(xPointer) * (xBobAmount * amountMultiplier), 0, 0) });
    }
}
