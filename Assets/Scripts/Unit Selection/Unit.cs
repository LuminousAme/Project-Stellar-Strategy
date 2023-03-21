using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    [SerializeField] UnitData unitData;

    private NavMeshAgent agent;

    private void Awake()
    {
        NavMeshHit closestPoint;
        if(NavMesh.SamplePosition(transform.position, out closestPoint, 1000, 1))
        {
            transform.position = closestPoint.position;
            agent = gameObject.AddComponent<NavMeshAgent>();
            agent.speed = unitData.maxSpeed;
        }
    }

    public void SetSeekTarget(Vector3 targetPosition)
    {
        targetPosition.y = transform.position.y;
        if (agent != null) agent.destination = targetPosition;
    }
}