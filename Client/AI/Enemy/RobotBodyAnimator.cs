using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Totalled;

public class RobotBodyAnimator : MonoBehaviour
{
    public Animator animator;
    public Robot robot;

    public float chasingMoveSpeedMultiplier;
    public float idleMoveSpeedMultiplier;
    float moveSpeedMultiplier;

    [HideInInspector] public float jumpingTime;
    public AnimationClip jumpAnimationClip;
    private void Update()
    {
        //animator.SetBool("Moving", robot.agent.velocity.magnitude > 0.2f);
        animator.SetFloat("Velocity", robot.agent.velocity.magnitude);
        moveSpeedMultiplier = 1f;
        switch (robot.currentActionState)
        {
            case RobotActionState.Idle:
                moveSpeedMultiplier = idleMoveSpeedMultiplier;
                break;
            case RobotActionState.Chasing:
                moveSpeedMultiplier = chasingMoveSpeedMultiplier;
                break;
            case RobotActionState.Attacking:
                moveSpeedMultiplier = chasingMoveSpeedMultiplier;
                break;
            case RobotActionState.Jumping:
                jumpingTime += Time.deltaTime;
                animator.SetBool("StartingJump", true);
                if (jumpingTime > robot.jumpingChargeTime){
                    animator.SetBool("Jumping", true);
                    if (jumpingTime > ((jumpAnimationClip.length / animator.GetCurrentAnimatorStateInfo(0).speed)+ robot.jumpingChargeTime)){
                        animator.SetBool("StartingJump", false);
                        animator.SetBool("Jumping", false);
                    }
                }
                break;
        }
        if (robot.canJump){
            if (robot.currentActionState != RobotActionState.Jumping){
                jumpingTime = 0f;
                animator.SetBool("StartingJump", false);
                animator.SetBool("Jumping", false);
            }
        }
        animator.SetFloat("MoveSpeed", robot.agent.velocity.magnitude* moveSpeedMultiplier);
        animator.SetBool("Stunned", !robot.stunRegaining);
    }
}
