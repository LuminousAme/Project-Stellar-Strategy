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

	Transform prevFollow;

	protected override void Update()
	{
		base.Update();

		if (orbitingPlanet && followTarget && followTarget.transform != orbitingPlanet.transform) {
			orbitingPlanet.StopGrabResources(this);
			orbitingPlanet = null;
		}
	}

	protected void OnTriggerStay(Collider other)
	{
		//look for what you're following
		if (followTarget && other.transform == followTarget.transform) {
			PlanetData data = followTarget.GetComponent<PlanetData>();

			if (data && !data.CheckExtractor(this)) {
				orbitingPlanet = data;
				orbitingPlanet.GrabResources(this);
			}

			//check if the station is connected
			StationUnit station = MatchManager.instance.stations[faction];
			if (followTarget == station.GetPlanet()) {
				float change = Mathf.Min(depositRate * Time.deltaTime, resourcesHeld);
				resourcesHeld -= change;
				station.DepositResources(change);
			}
		}
	}

	protected override void OnTriggerExit(Collider other)
	{
		base.OnTriggerExit(other);

		if (orbitingPlanet && other.transform == orbitingPlanet.transform) {
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

	public bool Unloading() {
		return orbitingPlanet && followTarget == MatchManager.instance.stations[faction].GetPlanet();
	}

	public float GetResources() => resourcesHeld;
	public float GetRate() => depositRate;
}
