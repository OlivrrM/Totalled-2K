using UnityEngine;

public class ArmIKController : MonoBehaviour
{
    public Animator animator;
    public Transform weapon; // Assign your weapon transform in the inspector
    public Transform handIKTarget; // Empty GameObject that represents where the hand should be

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            // Enable IK
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);

            // Set the IK position and rotation to match the weapon
            animator.SetIKPosition(AvatarIKGoal.RightHand, weapon.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, weapon.rotation);
        }
    }
}