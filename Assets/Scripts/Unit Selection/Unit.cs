using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

    private float cohesionRadius;
    private float alignRadius;

    public float maxSpeed = 10;
    public float maxForce = 20;

    //public GameObject unitSelect;
    [SerializeField]
    private UnitSelection unitSelector;
    private Rigidbody body;

    // Start is called before the first frame update
    void Start()
    {
        unitSelector = Object.FindObjectOfType<UnitSelection>();
       // maxSpeed = 10;
       // maxForce = 20;

}

    private void Awake()
    {
        body = gameObject.GetComponent<Rigidbody>();
        float radius = transform.localScale.x / 2;
        alignRadius = radius * radius + 2;

    }

    // Update is called once per frame
    void Update()
    {
     

    }

    public Vector3 Align()
    {
        Vector3 totalHeading = Vector3.zero;
        int numNeighbors = 0;

        // Loop through all boids in the world
        foreach (GameObject vehicle in unitSelector.selectedUnits)
        {
            Unit boid = vehicle.GetComponent<Unit>();

            Vector3 separationVector = transform.position - boid.transform.position;
            float distance = separationVector.magnitude;

            // If it's a neighbor within our vicinity
            if (distance > 0 && distance < alignRadius)
            {
                numNeighbors++;
                totalHeading += boid.body.velocity.normalized;
            }
        }

        // That is, if this boid actually has neighbors to worry about
        if (numNeighbors > 0)
        {
            // Average direction we need to head in
            Vector3 averageHeading = (totalHeading / numNeighbors);
            averageHeading.Normalize();

            // Compute the steering force we need to apply
            Vector3 alignmentForce = averageHeading * maxSpeed;

            // Cap that steering force
            if (alignmentForce.magnitude > maxForce)
            {
                alignmentForce.Normalize();
                alignmentForce *= maxForce;
            }

            //make sure it doenst go up
            alignmentForce.y = 0;

            return alignmentForce;
        }

        return Vector3.zero;
    }

    /*
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
    */

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
