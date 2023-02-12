using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CelestialBody : MonoBehaviour
{
    public CelestialBody rotatingAround;
    public float orbitTime = 60f;
    public bool flipDirection = false;
    float radius;
    float currentAngle;
    float orbitSpeed;

    void Start()
    {
        Init();
    }

    void OnValidate()
    {
        Init();
    }

    void Init()
    {
        if (rotatingAround)
        {
            Vector2 forward = new Vector2(rotatingAround.transform.forward.x, rotatingAround.transform.forward.z).normalized;
            Vector2 direction2D = GetDirection();
            currentAngle = Vector2.SignedAngle(forward, direction2D);
            while (currentAngle > 360f) currentAngle -= 360f;
            while (currentAngle < 0f) currentAngle += 360f;
            orbitSpeed = 360f / orbitTime;
        }
    }

    Vector2 GetDirection()
    {
        Vector3 direction3D = transform.position - rotatingAround.transform.position;
        return new Vector2(direction3D.x, direction3D.z).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        if (rotatingAround)
        {
            radius = Vector3.Distance(transform.position, rotatingAround.transform.position);
            if (!Application.isPlaying) Init();
            if(Application.isPlaying)
            {
                float flip = (flipDirection) ? 1f : -1f;
                currentAngle += flip * (orbitSpeed * Time.deltaTime); //modify time.delta to adjust for simulation speed later
                while (currentAngle > 360f) currentAngle -= 360f;
                while (currentAngle < 0f) currentAngle += 360f;
                Vector3 dir = Quaternion.AngleAxis(-currentAngle, Vector3.up) * rotatingAround.transform.forward;
                transform.position = rotatingAround.transform.position + (dir * radius);
            }
        }
    }

    [SerializeField] int GizmoSamples = 50;
    List<Vector3> points = new List<Vector3>();

    public void OnDrawGizmosSelected()
    {
        if (rotatingAround != null)
        {
            points.Add(rotatingAround.transform.position + (rotatingAround.transform.forward * radius));
            float baseOffset = 360f / (float)(GizmoSamples);
            for (int i = 1; i <= GizmoSamples; i++)
            {
                float offset = baseOffset * i;
                Vector3 dir = Quaternion.AngleAxis(offset, Vector3.up) * rotatingAround.transform.forward;
                points.Add(rotatingAround.transform.position + (dir * radius));
            }

            for (int i = 0; i + 1 < points.Count; i++)
                Gizmos.DrawLine(points[i], points[i + 1]);

            Gizmos.DrawLine(transform.position, rotatingAround.transform.position);

            float flip = (flipDirection) ? 1f : -1f;

            Vector2 direction2D = GetDirection();
            Vector3 direction3D = Quaternion.AngleAxis(-90.0f, Vector3.up) * new Vector3(direction2D.x, 0f, direction2D.y);
            direction3D *= flip;

            Gizmos.DrawLine(transform.position, transform.position + direction3D * 0.5f * radius);

            points.Clear();
        }
    }
}
