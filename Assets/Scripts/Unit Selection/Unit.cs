using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] UnitData unitData;

    private Collider[] objectsToAvoid;
    private float[] avoidValues = new float[8];

    private Vector3 targetPosition;
    private float[] seekValues = new float[8];

    private float[] moveValues = new float[8];
    Vector3 moveDirection = Vector3.zero;

    private float elapsedRotationTime = 0.0f;

    //public GameObject unitSelect;
    [SerializeField]
    private UnitSelection unitSelector;
    private Rigidbody body;

    // Start is called before the first frame update
    void Start()
    {
        unitSelector = FindObjectOfType<UnitSelection>();

        //initialize the arrays of seeks and avoidance values
        for(int i = 0; i < avoidValues.Length; i++)
        {
            avoidValues[i] = 0.0f;
            seekValues[i] = 0.0f;
            moveValues[i] = 0.0f;
        }

        targetPosition = transform.position;
        moveDirection = Vector3.zero;

        InvokeRepeating("Step", 0.0f, unitData.aiUpdateDelay);
}

    private void Awake()
    {
        body = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //remove any y component influence
        moveDirection.y = 0.0f;

        //get the t 
        float t = Mathf.Clamp01(elapsedRotationTime / unitData.fullRotationTime);

        Vector3 position = transform.position;
        position.y = 0.0f;
        if(Vector3.Distance(position, targetPosition) > unitData.acceptableSeekRadius)
        {
            //get the speed based on that t
            float speed = MathUtils.LerpClamped(0.0f, unitData.maxSpeed, t);
            //move based on the speed
            transform.position = transform.position + (moveDirection * unitData.maxSpeed * Time.deltaTime);
        }

        //calculate the angle
        /*
        float moveDirectionAngle = Vector3.SignedAngle(Vector3.forward, moveDirection, Vector3.up);
        float oppositeAngle = moveDirectionAngle + 180.0f;

        //rotate based on that angle
        Vector3 eulers = transform.rotation.eulerAngles;
        eulers.y = MathUtils.LerpClamped(oppositeAngle, moveDirectionAngle, t);
        transform.rotation = Quaternion.Euler(eulers);
        */

        //update the elapsed time
        if(elapsedRotationTime < unitData.fullRotationTime) elapsedRotationTime += Time.deltaTime;
    }

    private void Detect()
    {
        //use the layermask and a sphere cast to determine all of the objects that should currently be avoided
        Collider[] objectsToAvoid = Physics.OverlapSphere(transform.position, unitData.avoidanceRadius, unitData.avoidMask);
    }

    private void AvoidanceSteering()
    {
        //reset the values to zero
        for (int i = 0; i < Directions.EigthDirections.Length; i++)
        {
            avoidValues[i] = 0.0f;
        }

        if (objectsToAvoid == null) return;

        //iterate over all of the objects the unit is currently avoiding to determine what directions it should be avoiding
        for (int i = 0; i < objectsToAvoid.Length; i++)
        {
            Vector3 direction = objectsToAvoid[i].ClosestPoint(transform.position) - transform.position;
            direction.y = 0.0f; //ignore the y-axis
            float distance = direction.magnitude;

            //calculate the weight based on the distance
            float weight = distance <= unitData.unitRadius ? 1.0f : (unitData.avoidanceRadius - distance) / unitData.avoidanceRadius;

            direction = direction.normalized;

            for(int j = 0; j < Directions.EigthDirections.Length; j++ )
            {
                float result = Vector2.Dot(direction, Directions.EigthDirections[j]);
                float val = result * weight;

                if(val > avoidValues[j])
                {
                    avoidValues[j] = val;
                }
            }
        }
    }

    private void SeekSteering()
    {
        //reset the values to zero
        for (int i = 0; i < Directions.EigthDirections.Length; i++)
        {
            seekValues[i] = 0.0f;
        }

        //remove the affect of the y component
        Vector3 position = transform.position;
        position.y = 0.0f;
        targetPosition.y = 0.0f;
        if (Vector3.Distance(position, targetPosition) < unitData.acceptableSeekRadius)
        {
            return;
        }

        //otherwise try to get the seek values
        Vector3 directon = (targetPosition - position);
        directon.y = 0;
        directon = directon.normalized;
        for (int i = 0; i < Directions.EigthDirections.Length; i++)
        {
            float result = Vector3.Dot(directon, Directions.EigthDirections[i]);

            if(result > seekValues[i])
            {
                seekValues[i] = result;
            }
        }
    }

    private void CombineSteering()
    {
        //consider adding some condition to handle when the target point is right through an obstacle
        for(int i = 0; i < Directions.EigthDirections.Length; i++)
        {
            moveValues[i] = Mathf.Clamp01(seekValues[i] - avoidValues[i]);
        }

        Vector3 averageDirection = Vector3.zero;
        for(int i = 0; i < Directions.EigthDirections.Length; i++)
        {
            averageDirection += Directions.EigthDirections[i] * moveValues[i];
        }

        moveDirection = averageDirection.normalized;
    }

    private void Step()
    {
        Detect();
        AvoidanceSteering();
        SeekSteering();
        CombineSteering();

        moveDirection.y = 0f;
        moveDirection = moveDirection.normalized;

        Vector3 forward = transform.forward;
        forward.y = 0f;

        float dot = Vector3.Dot(moveDirection, forward);
        float t = MathUtils.ReMap(-1f, 1f, 0f, 1f, dot);
        elapsedRotationTime = MathUtils.LerpClamped(0.0f, unitData.fullRotationTime, t);
    }

    public void SetSeekTarget(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
        Step();
    }

    public static class Directions
    {
        public static Vector3[] EigthDirections =
        {
            (Vector3.forward).normalized,
            (Vector3.right + Vector3.forward).normalized,
            (Vector3.right).normalized,
            (Vector3.right + Vector3.back).normalized,
            (Vector3.back).normalized,
            (Vector3.left + Vector3.back).normalized,
            (Vector3.left).normalized,
            (Vector3.left + Vector3.forward).normalized
        };
    }
}