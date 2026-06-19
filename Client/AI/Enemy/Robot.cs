using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Totalled;
using UnityEngine.Rendering.Universal;
using TMPro;
//using Unity.Mathematics;

public class Robot : MonoBehaviour
{
    [Header("General")]
    public NavMeshAgent agent;
    public RobotBodyAnimator bodyAnimator;

    public RobotActionState currentActionState;

    Vector3 spawnPoint;
    Vector3 idlePoint;

    [HideInInspector] public Vector3 currentDesination;

    public float closestDistance;

    [HideInInspector] public Vector3 lastKnownTargetPos;

    [HideInInspector] public float distanceFromTarget;

    [Header("Idle")]
    public float maxIdleDistanceFromSpawn;

    public float idleRadius;
    public Vector2 idleWaitTimeAB;
    float currentIdleWaitTime;

    public float searchRadius;

    public float closestIdleDestinationDistance;
    [HideInInspector] public bool forceStandStill;

    public Vector3 lastReachablePos;
    bool didntFindReachablePosThisFrame;
    [HideInInspector] public float timeSinceLastReachablePos;

    [Header("Agro")]
    public float agroRadius;
    public float forceAgroRadius;
    public float chainAgroRadius;
    public bool staticAgroRadius;
    float chainAgroRadiusCooldown;
    float targetForceAgroRadius;
    [HideInInspector] public float currentForceAgroRadius;
    public float agroDetectionAngle;
    [HideInInspector] public float currentAgroRadius;
    float targetAgroRadius;
    public DecalProjector agroRadiusVisual;
    public DecalProjector forceAgroRadiusVisual;
    public static bool debugShowAgroRadius;
    public Transform head;
    public PlaySound agroSfx;
    bool agrod;
    public static bool ghost;
    [HideInInspector] public float damagedAgroMultiplier;
    [HideInInspector] public float alarmAgroMultiplier;
    [HideInInspector] public float chainAgroMultiplier;
    public float damageAgroMultiplierIncreaseAmount;
    [HideInInspector] public bool chasingLOS;
    [HideInInspector] public bool checkAttackingLOS;
    [HideInInspector] public bool attackingLOS;

    [Header("Backtracking")]
    public float backtrackFromAgrovatorDistance;
    public float backtrackOffset = 1.5f;
    public float backTrackingSpeedMultiplier;
    float currentBackTrackingSpeed;
    bool backTracking;

    [Header("Speed")]
    public float idleSpeed;
    public float chasingSpeed;
    [HideInInspector] public float baseSpeed;
    public RobotSpeedManager robotSpeedManager;

    [Header("Stun")]
    [HideInInspector] public bool stunRegaining;
    float currentStunRegainTime;
    float currentStunRegain;

    [Header("Attacking")]
    public float meleeDistance;
    public bool attacking;

    [Header("Jumping")]
    public bool canJump;
    bool queueJump;
    public float jumpDistance;
    [HideInInspector] public bool isJumping;
    int maxJumps = 5;
    int currentJumpCount = 0;
    public float jumpHeight;
    public float jumpCooldown;
    public float jumpDuration;
    Vector3 lastViablePlayerJumpPos;
    public float jumpingChargeTime;
    public float jumpLandTime;
    public PlaySound jumpStartSfx;
    public PlaySound jumpLandSfx;
    public float jumpSurroundingCheckCd = 1f;
    float currentJumpSurroundingCheckCd;

    [HideInInspector] public bool forceSearch;

    public bool manualControl;
    private void Start()
    {
        if (Client.online) { Destroy(gameObject); }

        spawnPoint = transform.position;
        idlePoint = spawnPoint;

        currentAgroRadius = agroRadius;

        agroRadiusVisual.gameObject.SetActive(debugShowAgroRadius);
        forceAgroRadiusVisual.gameObject.SetActive(debugShowAgroRadius);

        robotSpeedManager.AddValue("StunSpeed", 1f);
        currentStunRegain = 1f;

        alarmAgroMultiplier = 1f;
        chainAgroMultiplier = 1f;

        robotSpeedManager.AddValue("BacktrackSpeed", backTrackingSpeedMultiplier);

        stunRegaining = true;
    }
    private void OnEnable()
    {
        ResetSequenceOnRespawn.onSequenceReset += OnSequenceReset;
    }
    private void OnDisable()
    {
        ResetSequenceOnRespawn.onSequenceReset -= OnSequenceReset;
    }
    public void UpdateShowDebugAgroRadius()
    {
        agroRadiusVisual.gameObject.SetActive(debugShowAgroRadius);
        forceAgroRadiusVisual.gameObject.SetActive(debugShowAgroRadius);
    }
    private void Update()
    {
        chainAgroRadiusCooldown -= Time.deltaTime;
        if (didntFindReachablePosThisFrame) { timeSinceLastReachablePos += Time.deltaTime; }
        currentBackTrackingSpeed = Mathf.Lerp(currentBackTrackingSpeed, backTracking ? backTrackingSpeedMultiplier : 1f, Time.deltaTime * 2);
        backTracking = false;
        robotSpeedManager.UpdateValue("BacktrackSpeed", currentBackTrackingSpeed);
        if (!manualControl)
        {
            switch (currentActionState)
            {
                case RobotActionState.Idle:
                    bool reachedDestination = false;
                    queueJump = false;
                    if (closestIdleDestinationDistance != 0)
                    {
                        if (Vector3.Distance(transform.position, agent.destination) < closestIdleDestinationDistance)
                        {
                            reachedDestination = true;
                            SetDestination(transform.position);
                            forceStandStill = true;
                        }
                    }
                    if (ReachedDestination() || reachedDestination)
                    {
                        currentIdleWaitTime -= Time.deltaTime;
                        if (currentIdleWaitTime < 0f)
                        {
                            Idle();
                        }
                    }
                    break;
                case RobotActionState.Chasing:
                    if (forceSearch) { Search(); break; }
                    if (!Physics.Raycast(head.position, Cache.vcam.transform.position - head.position, Vector3.Distance(head.position, Cache.vcam.transform.position), Cache.references.solidLayerMask))
                    {
                        //Debug.DrawRay(head.position, (Cache.vcam.transform.position - head.position) * (Vector3.Distance(head.position, Cache.vcam.transform.position)), Color.red);
                        chasingLOS = true;
                        lastKnownTargetPos = Cache.surfCharacter.transform.position;
                        Agro();
                    }
                    else { chasingLOS = false; Search(); }
                    if (canJump)
                    {
                        if (distanceFromTarget < jumpDistance)
                        {
                            lastViablePlayerJumpPos = Cache.surfCharacter.transform.position;
                        }
                        if ((agent.pathStatus == NavMeshPathStatus.PathPartial || agent.pathStatus == NavMeshPathStatus.PathInvalid) || queueJump)
                        {
                            if (!isJumping)
                            {
                                TryFindJumpTarget();
                            }
                        }
                    }
                    break;
                case RobotActionState.Attacking:
                    if (forceSearch) { Search(); break; }
                    if (checkAttackingLOS)
                    {
                        if (!Physics.Raycast(head.position, Cache.vcam.transform.position - head.position, Vector3.Distance(head.position, Cache.vcam.transform.position), Cache.references.solidLayerMask))
                        {
                            //Debug.DrawRay(head.position, (Cache.vcam.transform.position - head.position) * (Vector3.Distance(head.position, Cache.vcam.transform.position)), Color.red);
                            attackingLOS = true;
                            lastKnownTargetPos = Cache.surfCharacter.transform.position;
                            Agro();
                        }
                        else { attackingLOS = false; }
                    }
                    //SetDestination(transform.position);
                    break;
                case RobotActionState.Custom:
                    break;
            }

            /*
            if (closestDistance != 0f){
                if (Vector3.Distance(transform.position, Cache.surfCharacter.transform.position) < closestDistance)
                {
                    currentIdleWaitTime -= Time.deltaTime;
                    if (currentIdleWaitTime < 0f)
                    {
                        SetDestination(transform.position);
                        currentIdleWaitTime = Random.RandomRange(idleWaitTimeAB.x, idleWaitTimeAB.y);
                    }
                }
            }
            */

            damagedAgroMultiplier = Mathf.Lerp(damagedAgroMultiplier, 1f, Time.deltaTime);
            alarmAgroMultiplier = Mathf.Lerp(alarmAgroMultiplier, 1f, Time.deltaTime / 2f);
            chainAgroMultiplier = Mathf.Lerp(chainAgroMultiplier, 1f, Time.deltaTime / 2f);

            CalculateAgroRadius();
            SetCurrentState();
        }

        if (stunRegaining){
            currentStunRegain = Mathf.Lerp(currentStunRegain, 1f, Time.deltaTime * currentStunRegainTime);
        }
        robotSpeedManager.UpdateValue("StunSpeed", currentStunRegain);
    }
    public void OnSequenceReset()
    {
        currentActionState = RobotActionState.Idle;
        SetDestination(transform.position);
    }
    void TryFindJumpTarget()
    {
        if (isJumping || !stunRegaining) return;

        float distanceToTarget = Vector3.Distance(transform.position, Cache.surfCharacter.transform.position);

        if (distanceToTarget <= jumpDistance)
        {
            StartCoroutine(PerformJump(Cache.surfCharacter.transform.position));
        }
        else
        {
            Vector3 bestJumpTarget = Vector3.zero;
            if (currentJumpSurroundingCheckCd <= 0f){
                bestJumpTarget = FindValidJumpSpotTowardsTarget();
                currentJumpSurroundingCheckCd = jumpSurroundingCheckCd;
            }
            else { currentJumpSurroundingCheckCd -= Time.deltaTime; }
            if (bestJumpTarget != Vector3.zero)
            {
                StartCoroutine(PerformJump(bestJumpTarget));
            }
        }
    }
    /*
    void TryFindJumpTarget()
    {
        if (currentJumpCount >= maxJumps)
        {
            Debug.Log("Max jump attempts reached. Cannot find path.");
            return; // Fallback behavior, e.g., give up chasing or move towards last known position
        }

        Vector3 direction = (Cache.surfCharacter.transform.position - transform.position).normalized;
        float jumpDistance = 3f; // Adjust based on jump capability

        Vector3 jumpTarget = transform.position + direction * jumpDistance;

        // Raycast to detect if the target is navigable
        NavMeshHit hit;
        if (NavMesh.SamplePosition(jumpTarget, out hit, 1.0f, NavMesh.AllAreas))
        {
            Debug.Log($"Jumping to position: {hit.position}");
            StartCoroutine(PerformJump(hit.position));
            currentJumpCount++;
        }
        else
        {
            // Try multiple angles if direct jump target is invalid
            bool jumpFound = false;
            for (int i = 1; i <= 8; i++)
            {
                float angle = i * 45f;
                Vector3 offset = Quaternion.Euler(0, angle, 0) * (direction * jumpDistance);
                jumpTarget = transform.position + offset;

                if (NavMesh.SamplePosition(jumpTarget, out hit, 1.0f, NavMesh.AllAreas))
                {
                    Debug.Log($"Jumping to alternate position: {hit.position}");
                    StartCoroutine(PerformJump(hit.position));
                    currentJumpCount++;
                    jumpFound = true;
                    break;
                }
            }

            if (!jumpFound)
            {
                Debug.LogWarning("No valid jump target found.");
            }
        }
    }
    */
    Vector3 FindValidJumpSpotTowardsTarget()
    {
        Vector3 playerPos = Cache.surfCharacter.transform.position;
        Vector3 directionToPlayer = (playerPos - transform.position).normalized;
        float currentDistance = Vector3.Distance(transform.position, playerPos);

        float searchRadius = jumpDistance;
        int sampleCount = 16;

        Vector3 bestPoint = Vector3.zero;
        float bestProgress = 0f;

        for (int i = 0; i < sampleCount; i++)
        {
            float angle = i * (360f / sampleCount);
            Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;

            // Try sampling a point in direction of player, and offset it downward
            Vector3 candidate = transform.position + dir * searchRadius;
            candidate += Vector3.down * 3f; // favor lower positions (tweak this value as needed)

            // Use a larger sample radius to better detect far-down surfaces
            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                float newDistance = Vector3.Distance(hit.position, playerPos);

                if (newDistance < currentDistance)
                {
                    float alignment = Vector3.Dot((hit.position - transform.position).normalized, directionToPlayer);
                    float progressScore = (currentDistance - newDistance) * alignment;

                    if (progressScore > bestProgress)
                    {
                        bestProgress = progressScore;
                        bestPoint = hit.position;
                    }
                }
            }
        }

        return bestPoint;
    }
    public IEnumerator PerformJump(Vector3 target)
    {
        queueJump = false;
        isJumping = true;
        agent.enabled = false; // Disable NavMeshAgent during the jump

        bodyAnimator.jumpingTime = 0f;

        jumpStartSfx.Play();

        yield return new WaitForSeconds(jumpingChargeTime);

        if (NavMesh.SamplePosition(target, out NavMeshHit hit, 5f, NavMesh.AllAreas)){
            target = hit.position;
        }

        Vector3 startPosition = transform.position;
        Vector3 jumpDirection = (target - startPosition).normalized;

        Quaternion endRot = agent.transform.rotation;
        agent.transform.LookAt(target);
        Quaternion startRot = agent.transform.rotation;

        float jumpDuration = 1f; // Duration of the jump
        float elapsedTime = 0f;

        while (elapsedTime < jumpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / jumpDuration;

            // Parabolic jump calculation
            float height = Mathf.Sin(t * Mathf.PI) * jumpHeight;
            transform.position = Vector3.Lerp(startPosition, target, t) + Vector3.up * height;
            transform.rotation = Quaternion.Lerp(startRot, endRot, t);

            yield return null;
        }

        transform.position = target;
        transform.rotation = endRot;

        agent.enabled = true; // Re-enable NavMeshAgent

        // Reset the jump count if path is now valid
        agent.SetDestination(Cache.surfCharacter.transform.position);
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(Cache.surfCharacter.transform.position, path);

        jumpLandSfx.Play();

        yield return new WaitForSeconds(jumpLandTime);

        //Put new speed multiplier here for smoother startoff

        isJumping = false;

        if (path.status == NavMeshPathStatus.PathPartial || path.status == NavMeshPathStatus.PathInvalid)
        {
            //Debug.Log("Path still invalid after jump. Attempting another jump.");
            TryFindJumpTarget();
        }
        else
        {
            //Debug.Log("Path found after jump.");
            currentJumpCount = 0; // Reset jump count if path is found
        }
    }
    
    void CalculateAgroRadius()
    {
        targetAgroRadius = agroRadius;
        targetForceAgroRadius = forceAgroRadius;
        if (!staticAgroRadius){
            if (FragMovementListener.hSpeedPercentage > 0.5f && !Cache.crouch.crouching) { targetAgroRadius *= 1.2f; targetForceAgroRadius *= 1.2f; }
            if (FragMovementListener.hSpeedPercentage > 1.2f) { targetAgroRadius *= 1.2f; targetForceAgroRadius *= 1.2f; }
            targetAgroRadius *= FirearmAgroManager.multiplier; targetForceAgroRadius *= (1 + ((FirearmAgroManager.multiplier - 1) * 2f));
            if (Cache.crouch.crouching) { targetForceAgroRadius *= 0.5f; }
            targetAgroRadius *= damagedAgroMultiplier * alarmAgroMultiplier * chainAgroMultiplier; targetForceAgroRadius *= damagedAgroMultiplier * alarmAgroMultiplier * chainAgroMultiplier;
            targetAgroRadius *= didntFindReachablePosThisFrame ? 2f : 1f;
            targetForceAgroRadius *= didntFindReachablePosThisFrame ? 2f : 1f;
        }

        currentAgroRadius = Mathf.Lerp(currentAgroRadius, targetAgroRadius, Time.deltaTime * 3f);
        currentForceAgroRadius = Mathf.Lerp(currentForceAgroRadius, targetForceAgroRadius, Time.deltaTime * 2.5f);

        if (agroRadiusVisual.enabled){
            float visualAgroAmount = currentActionState == RobotActionState.Idle ? currentAgroRadius : currentAgroRadius * 1.5f;
            agroRadiusVisual.size = new Vector3(visualAgroAmount * 2, visualAgroAmount * 2, 10f);
            forceAgroRadiusVisual.size = new Vector3(currentForceAgroRadius * 2, currentForceAgroRadius * 2, 10f);
        }
    }
    void SetCurrentState()
    {
        if (currentActionState == RobotActionState.Custom) { return; } // Let custom logic change back state itself
        if (isJumping) { currentActionState = RobotActionState.Jumping; return; }
        if (ghost) { currentActionState = RobotActionState.Idle; return; }
        distanceFromTarget = Vector3.Distance(transform.position, Cache.surfCharacter.transform.position);
        if (Death.dead) { currentActionState = RobotActionState.Idle; return; }
        if (distanceFromTarget < currentAgroRadius)
        {
            if (distanceFromTarget < currentForceAgroRadius){
                if (distanceFromTarget < meleeDistance) { 
                    currentActionState = RobotActionState.Attacking;
                    return;
                }
                currentActionState = RobotActionState.Chasing;
                return;
            }

            Vector3 directionToAgrovator = Cache.vcam.transform.position - head.position;
            directionToAgrovator.Normalize();
            float angle = Vector3.Angle(-head.forward, directionToAgrovator);
            if (angle <= agroDetectionAngle / 2){
                RaycastHit hit;
                if (!Physics.Raycast(head.position, directionToAgrovator,out hit, distanceFromTarget, Cache.references.solidLayerMask))
                {
                    //Debug.DrawRay(head.position, directionToAgrovator * (Vector3.Distance(head.position, Cache.vcam.transform.position)), Color.yellow);
                    currentActionState = RobotActionState.Chasing;
                    //Agro();
                    return;
                }
            }
        }
        else
        {
            if (currentActionState == RobotActionState.Chasing)
            {
                if (distanceFromTarget > currentAgroRadius * 1.5f)
                {
                    currentActionState = RobotActionState.Idle;
                    SetDestination(transform.position);
                    return;
                }
            }
        }
    }

    public void Idle()
    {
        agrod = false;
        baseSpeed = idleSpeed;
        Vector3 newDestination = new Vector3(transform.position.x+(Random.RandomRange(-idleRadius,idleRadius)),transform.position.y, transform.position.z + (Random.RandomRange(-idleRadius, idleRadius)));
        int PathsChecked = 0;
        NavMeshPath path = new NavMeshPath();
        while (true)
        {
            if (agent.CalculatePath(newDestination, path) && (Vector3.Distance(newDestination, idlePoint) < maxIdleDistanceFromSpawn) && (Vector3.Distance(transform.position,newDestination)>closestDistance))
            {
                SetDestination(new Vector3(newDestination.x, transform.position.y, newDestination.z));
                currentIdleWaitTime = Random.RandomRange(idleWaitTimeAB.x, idleWaitTimeAB.y);
                forceStandStill = false;
                break;
            }
            else
            {
                newDestination = new Vector3(transform.position.x + (Random.RandomRange(-idleRadius, idleRadius)), transform.position.y, transform.position.z + (Random.RandomRange(-idleRadius, idleRadius)));
            }
            PathsChecked++;
            if (PathsChecked > 100)
            {
                //print($"100 PATHS CHECKED FOR '{transform.name}': NONE FOUND");
                break;
            }
        }
    }
    public void Search()
    {
        if (ReachedDestination())
        {
            agrod = false;
            baseSpeed = chasingSpeed * 0.8f;
            Vector3 newDestination = new Vector3(transform.position.x + (Random.RandomRange(-searchRadius, searchRadius)), transform.position.y, transform.position.z + (Random.RandomRange(-searchRadius, searchRadius)));
            SetDestination(new Vector3(newDestination.x, transform.position.y, newDestination.z));
        }
    }
    public void ChainAgro(Transform agrovator)
    {
        if (chainAgroRadiusCooldown < 0f){
            chainAgroRadiusCooldown = 1f;
            Collider[] colliders = Physics.OverlapSphere(transform.position, chainAgroRadius, Cache.references.enemyLayerMask);
            foreach (Collider col in colliders){
                RobotBodyPart r = col.gameObject.GetComponent<RobotBodyPart>();
                if (r != null){
                    r.robot.robot.chainAgroMultiplier = 4f;
                    r.robot.robot.Agro(agrovator);
                    break;
                }
            }
        }
    }
    public void Agro(Transform agrovator = null)
    {
        agrovator ??= Cache.surfCharacter.transform;

        baseSpeed = chasingSpeed;
        idlePoint = transform.position;
        if (!agrod)
        {
            agroSfx.Play();
            ChainAgro(agrovator);
            agrod = true;
        }
        if (distanceFromTarget < backtrackFromAgrovatorDistance) 
        {
            BackTracking(agrovator.position);
        }
        else 
        {
            NavMeshPath path = new NavMeshPath();
            if (NavMesh.CalculatePath(agent.transform.position,agrovator.position,NavMesh.AllAreas,path)){
                bool pass = false;
                pass = IsPositionOnNavMeshIgnoreY(agrovator.position);
                if (path.status == NavMeshPathStatus.PathComplete) { pass = true; }
                if (pass){
                    lastReachablePos = agrovator.position;
                    SetDestination(agrovator.position);
                    didntFindReachablePosThisFrame = false;
                    timeSinceLastReachablePos = 0f;
                }
                else{
                    if (lastReachablePos != Vector3.zero) {
                        SetDestination(lastReachablePos);
                        didntFindReachablePosThisFrame = true;
                        if (timeSinceLastReachablePos > 1f){
                            queueJump = true;
                        }
                    }
                }
            }
            else{
                if (lastReachablePos != Vector3.zero) {
                    SetDestination(lastReachablePos);
                    didntFindReachablePosThisFrame = true;
                    if (timeSinceLastReachablePos > 1f){
                        queueJump = true;
                    }
                }
            }
        }
    }
    bool IsPositionOnNavMeshIgnoreY(Vector3 targetPosition, float maxDistance = 0.01f)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, maxDistance, NavMesh.AllAreas)){
            Vector3 targetFlat = new Vector3(targetPosition.x, 0, targetPosition.z);
            Vector3 hitFlat = new Vector3(hit.position.x, 0, hit.position.z);
            return Vector3.Distance(targetFlat, hitFlat) <= maxDistance;
        }
        return false;
    }
    public void BackTracking(Vector3 targetPos)
    {
        robotSpeedManager.UpdateValue("BacktrackSpeed", backTrackingSpeedMultiplier);
        Vector3 directionAway = (transform.position - targetPos).normalized;
        Vector3 sidewaysOffset = Vector3.Cross(directionAway, Vector3.up).normalized * backtrackOffset;
        Vector3 backtrackPosition = transform.position + directionAway + sidewaysOffset;
        agent.SetDestination(backtrackPosition);
        backTracking = true;


        //Vector3 directionToPlayer = (player.position - transform.position).normalized;
        //Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }
    public void Stun(float amount, float imobileTime,float regainSpeedTime)
    {
        currentStunRegainTime = regainSpeedTime;
        currentStunRegain = amount;
        if (!isJumping && imobileTime>0f)
        {
            StartCoroutine(StunTimer(imobileTime));
        }
    }
    IEnumerator StunTimer(float time)
    {
        stunRegaining = false;
        yield return new WaitForSeconds(time);
        stunRegaining = true;
    }
    public void SetDestination(Vector3 destination)
    {
        agent.SetDestination(destination);
        currentDesination = destination;
    }
    bool ReachedDestination()
    {
        if (!agent.pathPending){
            if (agent.isOnNavMesh)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
