using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShipUnit : Unit
{
    private NavMeshAgent agent;
    protected CelestialBody followTarget = null;

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

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
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
            if(agent != null && agent.isActiveAndEnabled) agent.destination = acutalTargetPosition;
        }
    }

    public virtual void SetSeekTarget(Vector3 targetPosition)
    {
        followTarget = null;
        targetPosition.y = transform.position.y;
        if (agent != null) agent.destination = targetPosition;
    }

    public virtual void SetFollowTarget(CelestialBody followTarget)
    {
        this.followTarget = followTarget;
    }

    public CelestialBody GetFollowTarget() {
        return followTarget;
    }
}
