using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationUnit : Unit
{
    [SerializeField] protected Vector3 offset;
    protected CelestialBody attachedPlanet;
    [SerializeField] protected float resources;
	protected List<Unit> managedUnits = new List<Unit>();
	public List<Unit> factionUnits {get => managedUnits;}
	public System.Action<Unit> onReceivedUnit;

    protected override void Update()
    {
        base.Update();
        if(attachedPlanet != null) transform.position = attachedPlanet.transform.position + offset;
        Select();
    }

    public void SetPlanet(CelestialBody planet)
    {
        attachedPlanet = planet;
        if (attachedPlanet != null) transform.position = attachedPlanet.transform.position + offset;
    }

    public CelestialBody GetPlanet() => attachedPlanet;

	public int GetUnitCount() => managedUnits.Count;

	public void AddUnit(Unit unit) {
		managedUnits.Add(unit);
		unit.OnUnitDestroyed += died => managedUnits.Remove(died);
		onReceivedUnit?.Invoke(unit);
	}

	public void DepositResources(float amt) {
		resources += amt;
	}

    public float GetResources() => resources;

    public bool TrySpendResources(float value, System.Type type)
    {
        if (resources >= value)
        {
			bool works = false;
			if (typeof(ShipUnit) == type) {
				works = MatchManager.instance.SpawnNewDestroyer(faction);
			}
			if (typeof(ExtractorUnit) == type) {
				works = MatchManager.instance.SpawnNewExtractor(faction);
			}
			if (works) {
            	resources -= value;
            	return true;
			}
        }
        return false;
    }

    public int GetOrbitingUnitCount<T>(CelestialBody followTarget = null, float orbitingRange = 100f) {
        int unitCount = 0;

        if (followTarget == null)
            followTarget = attachedPlanet;

        foreach (ShipUnit curUnit in managedUnits) {
            if (curUnit.GetType() == typeof(T) && curUnit.GetFollowTarget() == followTarget && Vector3.Distance(curUnit.transform.position, followTarget.transform.position) <= orbitingRange) {
                ++unitCount;
            }
        }
        return unitCount;
    }
}
