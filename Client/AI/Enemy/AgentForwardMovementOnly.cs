using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentForwardMovementOnly : MonoBehaviour
{
    public NavMeshAgent agent;
    public float rotationSpeedMultiplier = 1.0f; // Variable to control rotation speed

    [HideInInspector] public Vector3 crossProduct;

    void Start()
    {
        // Ensure the NavMeshAgent does not automatically rotate
        agent.updateRotation = false;
    }

    void Update()
    {
        // Check if the agent has a path and there are more than one corner
        if (agent.path.corners.Length > 1)
        {
            // Get the direction to the next corner
            Vector3 directionToNextCorner = (agent.path.corners[1] - transform.position).normalized;

            // Flatten the direction to the ground plane
            Vector3 flatDirection = new Vector3(directionToNextCorner.x, 0, directionToNextCorner.z).normalized;

            // Only proceed if there is a significant distance to the next corner
            if (flatDirection.magnitude > 0.1f)
            {
                // Calculate the desired target rotation
                Quaternion targetRotation = Quaternion.LookRotation(flatDirection);

                // Calculate the angular distance between the current and target rotation
                float angularDifference = Quaternion.Angle(transform.rotation, targetRotation);

                // Calculate the maximum rotation step allowed this frame
                float maxRotationStep = agent.speed * rotationSpeedMultiplier * Time.deltaTime;

                // Calculate the percentage of the rotation to apply
                float rotationFraction = Mathf.Min(1, maxRotationStep / angularDifference);

                // Apply the rotation with the calculated fraction
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationFraction);

                // Move the agent forward in its current direction
                agent.Move(transform.forward * agent.speed * Time.deltaTime);

                Vector3 forward = transform.forward;  // Current forward direction
                Vector3 toNextCorner = (agent.path.corners[1] - transform.position).normalized;  // Direction to next corner

                // Cross product between forward direction and direction to the next corner
                crossProduct = Vector3.Cross(forward, toNextCorner);
            }
        }
    }
}
