using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private Transform barrel;
    [SerializeField] private GameObject blastPrefab;
    [SerializeField] private float knockBackTime = 0.1f;
    [SerializeField] private float recoveryTime = 2f;
    [SerializeField] private float knockbackDistance = 0.1f;
    [SerializeField] private MultiParticle muzzleFlash;

    private Unit m_target;
    public Unit target
    {
        get { return m_target; }
        set { m_target = value; }
    }

    private Color color;
    public void SetColor(Color color) => this.color = color;

    float eplasedSinceLastFired = 0.0f;
    bool recovering = false;
    Vector3 barrelPos;

    private void Start()
    {
        eplasedSinceLastFired = 0.0f;
        recovering = false;
        barrelPos = barrel.position;
    }

    private void Update()
    {
        if (m_target == null) return;
        eplasedSinceLastFired += Time.deltaTime;

        Vector3 direction = (m_target.transform.position - transform.position);
        direction.y = 0f;
        direction = direction.normalized;

        Vector3 facing = transform.forward;
        Vector3 noYFacing = facing;
        noYFacing.y = 0f;

        float angle = Vector3.SignedAngle(noYFacing, direction, Vector3.up);

        transform.RotateAround(transform.position, transform.up, Mathf.Min(angle, rotationSpeed) * Time.deltaTime);

        if(angle < 5.0f && !recovering)
        {
            Fire();
        }

        Recover();
    }

    private void Fire()
    {
        eplasedSinceLastFired = 0.0f;

        muzzleFlash.SetColor(color);
        muzzleFlash.Play();

        GameObject go = Instantiate(blastPrefab, barrel.GetChild(0).position, transform.rotation);
        EnergyBlast blast = go.GetComponent<EnergyBlast>();
        blast.target = target;
        blast.SetColor(color);

        recovering = true;
    }

    private void Recover()
    {
        if (!recovering) return;

        Vector3 knockedBackPos = barrelPos + (barrel.forward * knockbackDistance);
        if (eplasedSinceLastFired <= knockBackTime)
        {
            float t = Mathf.Clamp01(eplasedSinceLastFired / knockBackTime);

            barrel.position = Vector3.Lerp(barrelPos, knockedBackPos, t);
        }
        else if (eplasedSinceLastFired < recoveryTime)
        {
            float t = Mathf.Clamp01((eplasedSinceLastFired - knockBackTime) / (knockBackTime * 0.5f));
            barrel.position = Vector3.Lerp(knockedBackPos, barrelPos, t);
        }
        else recovering = false;
    }
}
