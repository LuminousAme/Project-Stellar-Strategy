using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "CelestialConquest/UnitData", order = 1)]
public class UnitData : ScriptableObject
{
    [Header("Avoidance")]
    public float avoidanceRadius = 100.0f;

    public LayerMask avoidMask;

    public float unitRadius = 5.0f;

    [Header("Seeking")]
    public float acceptableSeekRadius = 10.0f;

    [Header("Other Controls")]
    public float aiUpdateDelay = 0.05f;

    public float maxSpeed = 10;
    public float fullRotationTime = 2.5f;
}
