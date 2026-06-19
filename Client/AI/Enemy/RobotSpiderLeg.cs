using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotSpiderLeg : MonoBehaviour
{
    public Robot robot;
    public Transform target;
    public Transform legTipOrigin;

    [Range(0, 1)]
    public float pinch;
    public float speed;
    public float height;
    public float legDistance;

    [Range(0, 1)]
    public float phaseOffset;
    float y;
    float pingPongTime;

    Vector3 pinchDistance;

    public AnimationCurve legVerticalMovement;
    public AnimationCurve legHorizontalMovement;

    public PlaySound clankSfx;
    bool clanked;
    public float clankSfxDelay;

    private void Update()
    {
        pingPongTime += Time.deltaTime * speed * robot.agent.velocity.magnitude;
        y = Mathf.PingPong(pingPongTime + phaseOffset, 1f);

        if (y > (0.5f - clankSfxDelay) && !clanked){
            clankSfx.Play();
            clanked = true;
        }
        else if (y < (0.5f- clankSfxDelay))
        {
            clanked = false;
        }

        pinchDistance = Vector3.Lerp(transform.position, legTipOrigin.position, pinch);
        Vector3 horizontal = Vector3.ClampMagnitude(robot.agent.velocity, legDistance) * legHorizontalMovement.Evaluate(y);
        float vertical = legVerticalMovement.Evaluate(y) * height * Mathf.Clamp01(robot.agent.velocity.magnitude);

        target.position = new Vector3(
            pinchDistance.x + horizontal.x,
            robot.transform.position.y + vertical,
            pinchDistance.z + horizontal.z
        );
    }
}
