using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "CelestialConquest/UnitData", order = 1)]
public class UnitData : ScriptableObject
{
    public float maxSpeed = 10f;
    public float followDistance = 10f;
    public float avoidanceRadius = 10f;
    public int baseHealth = 100;
    public float combatRadius = 20f;
}
