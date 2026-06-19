using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityManager : MonoBehaviour
{
    public struct velocityTimes
    {
        public Vector3 velocity;
        public float time;
    }
    public static Queue<velocityTimes> velocityHistory = new Queue<velocityTimes>();
    private void Awake()
    {
        Cache.velocityManager = this;
    }
    private void FixedUpdate()
    {
        velocityHistory.Enqueue(new velocityTimes { velocity = Cache.moveData.Velocity, time = Time.time }) ;
        while (velocityHistory.Count > 0 && Time.time - velocityHistory.Peek().time > 5f)
        {
            velocityHistory.Dequeue();
        }
    }
    public Vector3 GetVelocityFromPast(float secondsAgo)
    {
        float targetTime = Time.time - secondsAgo;

        velocityTimes closestVector = default;
        float closestTimeDifference = float.MaxValue;

        foreach (var vectorWithTime in velocityHistory)
        {
            float timeDifference = Mathf.Abs(vectorWithTime.time - targetTime);
            if (timeDifference < closestTimeDifference)
            {
                closestVector = vectorWithTime;
                closestTimeDifference = timeDifference;
            }
        }

        return closestVector.velocity;
    }
    public Vector3 PushTowards(Vector3 point)
    {
        Vector3 direction = (point - transform.position).normalized;
        return direction;
    }
    public Vector3 ChangeDirection(Vector3 currentVelocity, Vector3 newDirection)
    {
        return currentVelocity.magnitude * newDirection.normalized;
    }
}
