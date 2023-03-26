using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtractorUnit : ShipUnit
{
	[SerializeField]	float resourcesHeld = 0f;
	[SerializeField]	float maxResourcesHeld = 10000f;
	[SerializeField]	float depositRate = 1000f;

	PlanetData orbitingPlanet;

	protected override void Start()
	{
		base.Start();

		OnUnitDestroyed += unit => {
			if (orbitingPlanet) {
				orbitingPlanet.StopGrabResources(this);
			}
		};
	}

	protected override void Update()
	{
		base.Update();

		if (orbitingPlanet && followTarget != orbitingPlanet.transform) {
			orbitingPlanet.StopGrabResources(this);
		}
	}

	protected override void OnTriggerEnter(Collider other)
	{
		base.OnTriggerEnter(other);

		//if in range to target
		if (other.transform == followTarget) {
			PlanetData data = followTarget.GetComponent<PlanetData>();

			if (data) {
				orbitingPlanet = data;
				orbitingPlanet.GrabResources(this);
				return;
			}

			//if not a planet, check if it's the station
			StationUnit station = MatchManager.instance.stations[faction];
			if (followTarget == station.transform) {
				float change = Mathf.Min(depositRate * Time.deltaTime, resourcesHeld);
				resourcesHeld -= change;
				station.DepositResources(change);
			}
		}
	}

	protected override void OnTriggerExit(Collider other)
	{
		base.OnTriggerExit(other);

		if (other.transform == orbitingPlanet.transform) {
			orbitingPlanet.StopGrabResources(this);
			orbitingPlanet = null;
		}
	}

	public void GiveResources(float amt) {
		resourcesHeld = Mathf.Min(resourcesHeld + amt, maxResourcesHeld);

		if (resourcesHeld == maxResourcesHeld) {
			//do something
			SetFollowTarget(MatchManager.instance.stations[faction]?.GetPlanet());
		}
	}
}
