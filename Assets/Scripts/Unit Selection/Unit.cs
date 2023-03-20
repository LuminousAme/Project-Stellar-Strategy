using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

    private float cohesionRadius;

    public float maxSpeed = 90;
    public float maxForce = 100;

    //public GameObject unitSelect;
    [SerializeField]
    private UnitSelection unitSelector;
    private Rigidbody body;

    // Start is called before the first frame update
    void Start()
    {
        unitSelector = Object.FindObjectOfType<UnitSelection>();

    }

    private void Awake()
    {
        body = gameObject.GetComponent<Rigidbody>();
        float radius = transform.localScale.x / 2;
        cohesionRadius = radius * radius + 2;

    }

    // Update is called once per frame
    void Update()
    {
     

    }

    public Vector3 Cohere()
    {
        Vector3 totalPositions = Vector3.zero;
        int numNeighbors = 0;

        // Loop through all boids in the world
        foreach (GameObject vehicle in unitSelector.selectedUnits)
        {
            Unit boid = vehicle.GetComponent<Unit>();

            Vector3 separationVector = transform.position - boid.transform.position;
            float distance = separationVector.magnitude;

            // If it's a neighbor within our vicinity, add its position to cumulative
            if (distance > 0 && distance < cohesionRadius)
            {
                numNeighbors++;
                totalPositions += boid.body.velocity.normalized;
            }
        }

        // If there are neighbors
        if (numNeighbors > 0)
        {
            Vector3 averagePosition = (totalPositions / numNeighbors);
            return Seek(averagePosition);
        }

        return Vector3.zero;
    }


    public Vector3 Seek(Vector3 target)
    {
        // Force to be applied to the boid
        Vector3 steerForce = ((target - transform.position).normalized * maxSpeed) - body.velocity;

        // Cap the force that can be applied
        if (steerForce.magnitude > maxForce)
        {
            steerForce = steerForce.normalized * maxForce;
        }

        return steerForce;
    }




}
