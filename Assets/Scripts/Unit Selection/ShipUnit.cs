using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShipUnit : Unit
{
    private NavMeshAgent agent;
    CelestialBody followTarget = null;

    private void Awake()
    {
        NavMeshHit closestPoint;
        if (NavMesh.SamplePosition(transform.position, out closestPoint, 1000, 1))
        {
            transform.position = closestPoint.position;
            agent = gameObject.AddComponent<NavMeshAgent>();
            agent.speed = unitData.maxSpeed;
            agent.radius = unitData.avoidanceRadius;
        }
    }

    private void Update()
    {
        if(followTarget != null)
        {
            Vector3 targetPosition = followTarget.transform.position;
            targetPosition.y = 0.0f;
            Vector3 position = transform.position;
            position.y = 0.0f;

            Vector3 inverseDirection = (position - targetPosition).normalized;
            inverseDirection *= unitData.followDistance;

            Vector3 acutalTargetPosition = targetPosition + inverseDirection;
            acutalTargetPosition.y = transform.position.y;
            if(agent != null) agent.destination = acutalTargetPosition;
        }
    }

    public void SetSeekTarget(Vector3 targetPosition)
    {
        followTarget = null;
        targetPosition.y = transform.position.y;
        if (agent != null) agent.destination = targetPosition;
    }

    public void SetFollowTarget(CelestialBody followTarget)
    {
        this.followTarget = followTarget;
    }


}
